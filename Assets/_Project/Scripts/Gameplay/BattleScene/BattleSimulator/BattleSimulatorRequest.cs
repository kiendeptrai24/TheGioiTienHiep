

using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BattleSimulatorRequest : SingletonNetwork<BattleSimulatorRequest>
{
    public List<BattleEvent> battleEvents = new();
    public BattleHistoryController battleHistoryController;
    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
        battleHistoryController = GetComponent<BattleHistoryController>();
    }
    private void RequestBattleSimulator(ulong playerClientId, ulong monsterNetId)
    {
        if (!IsServer) return;
        if (!NetworkManager.ConnectedClients.TryGetValue(playerClientId, out var playerNet)) return;
        if (!NetworkManager.SpawnManager.SpawnedObjects
            .TryGetValue(monsterNetId, out var enemyNO))
            return;

        var enemyObj = enemyNO.gameObject;
        var playerObj = playerNet.PlayerObject;

        if (playerObj == null) return;
        if (enemyObj == null) return;

        var heroRoster = playerObj.GetComponent<PlayerBattleRoster>();
        var enemyRoster = enemyObj.GetComponent<PlayerBattleRoster>();

        List<UnitInput> enemySnaps = new();
        List<UnitInput> heroSnaps = new();
        Board board = new Board
        {
            width = 5,
            height = 9,
            moveInterval = .3f,
            allowDiagonal = true
        };

        BattleBoardGrid boardGrid = new BattleBoardGrid(board.moveInterval, board.allowDiagonal);

        // HERO
        foreach (var heroPrefab in heroRoster.chamPrefabs)
        {
            if (heroPrefab == null) continue;

            var stats = heroPrefab.GetComponent<StatsData>();
            stats.SetUpItem(stats.heroData);
            var snap = SnapshotMapper.FromStats(stats, TeamId.Heroes);
            var pos = (stats.heroData as HeroData).championIndex;
            pos = boardGrid.ClampToValidCell(pos);

            snap.placement.cell = pos;
            snap.placement.attackRange = (int)snap.snap.attackRange;
            heroSnaps.Add(snap);
        }

        // ENEMY
        foreach (var enemyPrefab in enemyRoster.chamPrefabs)
        {
            if (enemyPrefab == null) continue;

            var stats = enemyPrefab.GetComponent<StatsData>();
            stats.SetUpItem(stats.heroData);

            var snap = SnapshotMapper.FromStats(stats, TeamId.Enemies);

            Vector2Int pos = (stats.heroData as HeroData).championIndex;
            pos.x = board.width - 1 - pos.x;
            pos.y = board.height - 1 - pos.y;
            pos = boardGrid.ClampToValidCell(pos);

            snap.placement.cell = pos;
            snap.placement.attackRange = (int)snap.snap.attackRange;
            enemySnaps.Add(snap);
        }



        uint seed = (uint)(playerClientId.GetHashCode() ^ monsterNetId.GetHashCode() ^ Environment.TickCount);


        var res = BattleSimulator.Simulate(heroSnaps, enemySnaps, seed, boardGrid, 60f);
        // convert to DTO
        var dto = new BattleEventDTO[res.events.Count];
        for (int i = 0; i < res.events.Count; i++)
        {
            var ev = res.events[i];
            dto[i] = BattleEventMapper.ToDTO(ev);
        }

        SendReplayToClientClientRpc(heroRoster.name, enemyRoster.name,
            res.winner.ToString(), res.duration, dto,
            new ClientRpcParams
            {
                Send = new ClientRpcSendParams { TargetClientIds = new[] { playerClientId } }
            });
    }
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void RequestBattleSimulatorServerRpc(ulong playerClientId, ulong monsterNetId)
    {
        RequestBattleSimulator(playerClientId, monsterNetId);
    }
    [ClientRpc]
    private void SendReplayToClientClientRpc(string namePlayer, string nameEnemy, string winner, float duration, BattleEventDTO[] events, ClientRpcParams rpcParams = default)
    {
        Debug.Log($"Đội chiến thắng là: {winner} với thời gian {duration} giây");
        string text = "";
        // convert to Data
        List<BattleEvent> battleEvents = new List<BattleEvent>();
        foreach (var dto in events)
        {
            battleEvents.Add(BattleEventMapper.FromDTO(dto));
        }
        this.battleEvents = battleEvents;

        BattleHistory battleHistory = new BattleHistory();
        battleHistory.winner = winner;
        battleHistory.duration = duration;
        battleHistory.name = namePlayer + "/" + nameEnemy;
        battleHistory.namePlayer = namePlayer;
        battleHistory.nameEnemy = nameEnemy;
        battleHistory.dateTime = DateTime.Now;
        battleHistory.battleEvents = battleEvents;

        battleHistoryController.AddBattleHistory(battleHistory);

        for (int i = 0; i < battleEvents.Count; i++)
        {
            var ev = battleEvents[i];

            switch (ev)
            {
                case BattleEventMove m:
                    text += $"{m.time:0.00}s MOVE uid: {m.ownerUid} team {m.team} {m.from} -> {m.to} to {m.targetUid}\n\n";
                    break;

                case BattleEventSkill s:
                    text += $"{s.time:0.00}s SKILL uid: {s.ownerUid} team {s.team} enemyTeam {s.targetTeam} -> {s.targetUid} " +
                            $"skillId={s.skillId} dmg={s.damage} crit={s.isCrit} hpAfter={s.targetHpAfter}\n\n";
                    break;
                case BattleEventAttack a:
                    text += $"{a.time:0.00}s ATK uid: {a.ownerUid} team {a.team} enemyTeam {a.targetTeam} -> uid: {a.targetUid}" +
                            $"dmg={a.damage} crit={a.isCrit} hpAfter={a.targetHpAfter}\n\n";
                    break;
                case BattleEventDealth d:
                    text += $"{d.time:0.00}s (DEATH uid: {d.ownerUid} team: {d.team}) attackerTeam: {d.attackerTeam} uid: {d.attackerUid} killed -> targetTeam: {d.targetTeam} uid: {d.targetUid} \n\n";
                    break;
                case BattleEventInit b:
                    text += $"{b.time:0.00}s champion init with uid{b.ownerUid} team {b.team} maxHp {b.maxHp} curHp {b.curtHp}\n\n";
                    break;
                default:
                    text += $"{ev.time:0.00}s {ev.type} uid: {ev.ownerUid}\n\n";
                    break;
            }
        }

        Debug.Log(text);
    }

}