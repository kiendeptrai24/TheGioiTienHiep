using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BattleService : NetworkBehaviour
{
    public static BattleService Instance { get; private set; }

    [Header("Battle islands in scene")]
    [SerializeField] private BattleIsland[] islands;

    private int _nextSessionId = 1;
    private int _nextIslandIndex = 0;

    private readonly Dictionary<int, BattleSession> _sessions = new();

    private void Awake() => Instance = this;

    public void ServerStartBattle(ulong playerClientId, ulong monsterNetId, MonsterBattleLoadout loadout)
    {
        if (!IsServer) return;

        if (!NetworkManager.ConnectedClients.TryGetValue(playerClientId, out var cc)) return;
        var playerObj = cc.PlayerObject;
        if (playerObj == null) return;

        var st = playerObj.GetComponent<PlayerBattleState>();
        if (st == null || st.InBattle.Value) return;

        int sessionId = _nextSessionId++;
        var island = AllocateIsland();
        if (island == null) return;

        // đánh dấu player vào battle (player đứng yên)
        st.ServerEnterBattle(sessionId, monsterNetId);

        var session = new BattleSession(sessionId, playerClientId, monsterNetId, island);
        _sessions[sessionId] = session;
        // spawn heroes từ player roster
        SpawnHeroes(session, playerObj);

        // spawn enemies từ monster loadout
        SpawnEnemies(session, loadout);

        // báo client bật UI + chuyển camera nhìn island
        NotifyEnterBattleClientRpc(sessionId, island.transform.position, new ClientRpcParams
        {
            Send = new ClientRpcSendParams { TargetClientIds = new[] { playerClientId } }
        });
    }

    private BattleIsland AllocateIsland()
    {
        if (islands == null || islands.Length == 0) return null;
        var isl = islands[_nextIslandIndex % islands.Length];
        _nextIslandIndex++;
        return isl;
    }

    private void SpawnHeroes(BattleSession s, NetworkObject playerObj)
    {
        var roster = playerObj.GetComponent<PlayerBattleRoster>();
        if (roster == null) return;

        int n = Mathf.Min(roster.maxHeroesToSpawn, roster.heroPrefabs.Count);
        for (int i = 0; i < n; i++)
        {
            var prefab = roster.heroPrefabs[i];
            if (prefab == null) continue;

            var pos = GetSpawn(s.Island.heroSpawns, i, s.Island.transform.position);
            var hero = Instantiate(prefab, pos, Quaternion.identity);

            var vis = hero.GetComponent<SessionVisibility>();
            if (vis != null) vis.SessionId = s.SessionId;

            var heroController = hero.GetComponent<HeroController>();
            if (heroController != null) heroController.SetTeamId(0);
            heroController.target.battleState = true;

            hero.Spawn(true); // server-owned
            s.SpawnedObjects.Add(hero);
        }
    }

    private void SpawnEnemies(BattleSession s, MonsterBattleLoadout loadout)
    {
        for (int i = 0; i < loadout.enemyPrefabs.Count; i++)
        {
            var prefab = loadout.enemyPrefabs[i];
            if (prefab == null) continue;

            var pos = GetSpawn(s.Island.enemySpawns, i, s.Island.transform.position);
            var enemy = Instantiate(prefab, pos, Quaternion.identity);

            var vis = enemy.GetComponent<SessionVisibility>();
            if (vis != null) vis.SessionId = s.SessionId;

            // Optional: gắn component BattleEnemy để khi chết gọi EndBattle
            var be = enemy.GetComponent<BattleEnemy>();
            if (be != null) be.SessionId = s.SessionId;

            var enemyController = enemy.GetComponent<HeroController>();
            if (enemyController != null) enemyController.SetTeamId(1);
            enemyController.target.battleState = true;

            enemy.Spawn(true);
            s.SpawnedObjects.Add(enemy);
            s.EnemyCount++;
        }
    }

    private Vector3 GetSpawn(Transform[] slots, int index, Vector3 islandPos)
    {
        if (slots != null && slots.Length > 0)
            return islandPos + slots[Mathf.Min(index, slots.Length - 1)].localPosition;

        // fallback
        return islandPos + new Vector3(index * 1.5f, 0f, 0f);
    }

    public void ServerOnEnemyDead(int sessionId)
    {
        if (!IsServer) return;
        if (!_sessions.TryGetValue(sessionId, out var s)) return;

        s.EnemyCount--;
        if (s.EnemyCount > 0) return;

        // hết enemy -> end battle
        ServerEndBattle(sessionId);
    }

    public void ServerEndBattle(int sessionId)
    {
        if (!IsServer) return;
        if (!_sessions.TryGetValue(sessionId, out var s)) return;

        // despawn battle objects
        foreach (var no in s.SpawnedObjects)
        {
            if (no != null && no.IsSpawned) no.Despawn(true);
        }

        // trả player về world state (player vẫn đang đứng yên ở chỗ cũ)
        if (NetworkManager.ConnectedClients.TryGetValue(s.PlayerClientId, out var cc) && cc.PlayerObject != null)
        {
            var st = cc.PlayerObject.GetComponent<PlayerBattleState>();
            st?.ServerExitBattle();

            NotifyExitBattleClientRpc(new ClientRpcParams
            {
                Send = new ClientRpcSendParams { TargetClientIds = new[] { s.PlayerClientId } }
            });
        }

        _sessions.Remove(sessionId);
    }

    [ClientRpc]
    private void NotifyEnterBattleClientRpc(int sessionId, Vector3 islandWorldPos, ClientRpcParams rpcParams = default)
    {
        BattleUIController.Instance?.EnterBattle(sessionId);
        BattleCameraController.Instance?.LookAtBattle(islandWorldPos);
    }

    [ClientRpc]
    private void NotifyExitBattleClientRpc(ClientRpcParams rpcParams = default)
    {
        BattleUIController.Instance?.ExitBattle();
        BattleCameraController.Instance?.ReturnToWorld();
    }
}

public class BattleSession
{
    public int SessionId;
    public ulong PlayerClientId;
    public ulong MonsterNetId;
    public BattleIsland Island;

    public int EnemyCount;
    public readonly List<NetworkObject> SpawnedObjects = new();

    public BattleSession(int id, ulong playerCid, ulong monsterNetId, BattleIsland island)
    {
        SessionId = id;
        PlayerClientId = playerCid;
        MonsterNetId = monsterNetId;
        Island = island;
    }
}
