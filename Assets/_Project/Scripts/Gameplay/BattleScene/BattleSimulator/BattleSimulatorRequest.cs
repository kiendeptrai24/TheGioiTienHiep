

using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BattleSimulatorRequest : SingletonNetwork<BattleSimulatorRequest>
{

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

        // HERO uid: 0..H-1
        int uid = 1;
        foreach (var heroPrefab in heroRoster.heroPrefabs)
        {
            if (heroPrefab == null) continue;

            // nếu đây là prefab (NetworkObject) -> phải instantiate để lấy StatsData runtime
            var stats = heroPrefab.GetComponent<StatsData>();
            stats.Setup(); // hoặc SetupDataPreset/Setup tùy bạn

            heroSnaps.Add(SnapshotMapper.FromStats(stats, uid, TeamId.Heroes));
            uid++;
        }

        int heroCount = 101;

        // ENEMY uid: heroCount..heroCount+E-1
        foreach (var enemyPrefab in enemyRoster.heroPrefabs)
        {
            if (enemyPrefab == null) continue;

            var stats = enemyPrefab.GetComponent<StatsData>();
            stats.Setup();

            enemySnaps.Add(SnapshotMapper.FromStats(stats, heroCount, TeamId.Enemies));
            heroCount++;
        }

        uint seed = (uint)(playerClientId.GetHashCode() ^ monsterNetId.GetHashCode() ^ Environment.TickCount);
        var res = BattleSimulator.Simulate(heroSnaps, enemySnaps, seed, 60f);

        var dto = new BattleEventDTO[res.events.Count];
        for (int i = 0; i < res.events.Count; i++)
        {
            var ev = res.events[i];
            dto[i] = new BattleEventDTO
            {
                t = ev.t,
                type = ev.type,
                attackerUid = ev.attackerUid,
                targetUid = ev.targetUid,
                damage = ev.damage,
                isCrit = ev.isCrit,
                targetHpAfter = ev.targetHpAfter,
                skillType = ev.skillType,
                skillIndex = ev.skillIndex
            };
        }

        SendReplayToClientClientRpc(res.winner.ToString(), res.duration, dto,
            new ClientRpcParams
            {
                Send = new ClientRpcSendParams { TargetClientIds = new[] { playerClientId } }
            });
    }
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    public void RequestBattleSimulatorServerRpc(ulong playerClientId, ulong monsterNetId)
    {
        RequestBattleSimulator(playerClientId, monsterNetId);
    }
    [ClientRpc]
    private void SendReplayToClientClientRpc(string winner, float duration, BattleEventDTO[] events, ClientRpcParams rpcParams = default)
    {
        Debug.Log($"Đội chiến thắng là: {winner} với thời gian {duration} giây");
        string text = "";
        for (int i = 0; i < events.Length; i++)
        {
            text += $"{events[i].t}: {events[i].type} {events[i].attackerUid} {events[i].targetUid} {events[i].damage} {events[i].isCrit} {events[i].targetHpAfter} \n";
        }
        Debug.Log(text);
    }

}