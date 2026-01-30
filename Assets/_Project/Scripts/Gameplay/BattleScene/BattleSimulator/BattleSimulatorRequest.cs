

using System;
using System.Collections.Generic;
using System.Linq;
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

        List<UnitSnapshot> enemySnaps = new();
        List<UnitSnapshot> heroSnaps = new();

        foreach (var enemy in enemyRoster.heroPrefabs)
        {
            enemy.GetComponent<StatsData>().SetupDataPreset();
            var unitSnapshot = SnapshotMapper.FromStats(enemy.GetComponent<StatsData>(), 0, TeamId.Enemies);
            enemySnaps.Add(unitSnapshot);
        }
        foreach (var hero in heroRoster.heroPrefabs)
        {
            hero.GetComponent<StatsData>().SetupDataPreset();
            var unitSnapshot = SnapshotMapper.FromStats(hero.GetComponent<StatsData>(), 0, TeamId.Heroes);
            heroSnaps.Add(unitSnapshot);
        }
        uint seed = (uint)(playerClientId.GetHashCode() ^ monsterNetId.GetHashCode() ^ Environment.TickCount);
        var res = BattleSimulator.Simulate(heroSnaps, enemySnaps, seed, 60f);

        List<BattleEventDTO> events = new();

        foreach (var @event in res.events)
        {
            events.Add(new BattleEventDTO
            {
                t = @event.t,
                type = @event.type,
                attackerUid = @event.attackerUid,
                targetUid = @event.targetUid,
                damage = @event.damage,
                isCrit = @event.isCrit,
                targetHpAfter = @event.targetHpAfter
            });
        }
        SendReplayToClientClientRpc(res.winner.ToString(), res.duration, events.ToArray(),
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