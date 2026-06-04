

using System;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Unity.Netcode;
using UnityEngine;

public class BattleSimulatorRequest : SingletonNetwork<BattleSimulatorRequest>
{
    public List<BattleEvent> battleEvents = new();
    public BattleHistoryController battleHistoryController;
    private StatsDataCore stats;
    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
        stats = new StatsDataCore(null);
        List<IStatsModifier> statsModifiers = new();
        statsModifiers.Add(new StatsCharacterModifier());
        statsModifiers.Add(new StatsRealmModifier());
        statsModifiers.Add(new StatsEssenceModifier());
        statsModifiers.Add(new StatsRaceModifier());
        statsModifiers.Add(new StatsEquipmentModifier());
        statsModifiers.Add(new StatsTechniqueModifier());
        statsModifiers.Add(new StatsSkillModifier());
        stats.SetStatsModifier(statsModifiers);
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
        battleHistoryController = GetComponent<BattleHistoryController>();
    }
    public void RequestBattleSimulator(ulong playerNetId, ulong monsterNetId, Action<bool> result = default)
    {
        if (!IsServer) return;

        if (!NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(playerNetId, out var playerNet))
            return;
        if (!NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(monsterNetId, out var enemyNO))
            return;
        var enemyObj = enemyNO.gameObject;
        var playerObj = playerNet.gameObject;
        if (playerObj == null || enemyObj == null) return;

        var senderNet = playerObj.GetComponent<NetworkBehaviour>();

        if (senderNet == null) return;

        ulong senderClientId = senderNet.OwnerClientId;

        if (playerObj == null) return;
        if (enemyObj == null) return;

        var heroRoster = playerObj.GetComponent<PlayerBattleRoster>();
        var enemyRoster = enemyObj.GetComponent<PlayerBattleRoster>();

        float heroHealthPersent = GetHealthPercent(heroRoster.itemDatas);
        float enemyHealthPersent = GetHealthPercent(enemyRoster.itemDatas);

        float heroManaPersent = GetManaPercent(heroRoster.itemDatas);
        float enemyManaPersent = GetManaPercent(enemyRoster.itemDatas);

        float heroSpiritPersent = GetSpiritPercent(heroRoster.itemDatas);
        float enemySpiritPersent = GetSpiritPercent(enemyRoster.itemDatas);

        List<UnitInput> enemySnaps = new();
        List<UnitInput> heroSnaps = new();
        Board board = new Board
        {
            width = 5,
            height = 9,
            moveInterval = 1f,
            allowDiagonal = true
        };

        BattleBoardGrid boardGrid = new BattleBoardGrid(board.moveInterval, board.allowDiagonal);
        // HERO
        foreach (var itemData in heroRoster.itemDatas)
        {
            if (itemData == null) continue;
            stats.SetUp(itemData);
            var snap = SnapshotMapper.FromStats(stats, TeamId.Heroes, heroHealthPersent, heroManaPersent, heroHealthPersent);

            Vector2Int pos = (stats.heroData as HeroData).championIndex;
            pos = boardGrid.ClampToValidCell(pos);

            snap.placement.cell = pos;
            snap.placement.attackRange = (int)snap.snap.attackRange;
            heroSnaps.Add(snap);
        }

        // ENEMY
        foreach (var itemData in enemyRoster.itemDatas)
        {
            if (itemData == null) continue;
            stats.SetUp(itemData);

            var snap = SnapshotMapper.FromStats(stats, TeamId.Enemies, enemyHealthPersent, enemyManaPersent, enemyHealthPersent);

            Vector2Int pos = (stats.heroData as HeroData).championIndex;

            pos.x = board.width - 1 - pos.x;
            pos.y = board.height - 1 - pos.y;
            pos = boardGrid.ClampToValidCell(pos);

            snap.placement.cell = pos;
            snap.placement.attackRange = (int)snap.snap.attackRange;
            enemySnaps.Add(snap);
        }

        uint seed = (uint)(playerNetId.GetHashCode() ^ monsterNetId.GetHashCode() ^ Environment.TickCount);


        var res = BattleSimulator.Simulate(heroSnaps, enemySnaps, seed, boardGrid, 60f);
        // convert to DTO
        var dto = new BattleEventDTO[res.events.Count];
        for (int i = 0; i < res.events.Count; i++)
        {
            var ev = res.events[i];
            dto[i] = BattleEventMapper.ToDTO(ev);
        }
        var playerHealth = playerNet.gameObject.GetComponent<PlayerVitals>();
        var enemyHealth = enemyNO.gameObject.GetComponent<PlayerVitals>();
        if (res.winner == TeamId.Heroes)
        {
            if (playerNet.IsPlayerObject)
            {
                if (playerHealth != null)
                {
                    ApplyCharacterViralRatioFromBattle(res.events, playerHealth, res.winner);
                }
            }
            if (enemyNO.IsPlayerObject)
                enemyHealth.ResetViral();
        }
        else
        {
            if (enemyNO.IsPlayerObject)
            {
                if (enemyHealth != null)
                {
                    ApplyCharacterViralRatioFromBattle(res.events, enemyHealth, res.winner);
                }
            }
            if (playerNet.IsPlayerObject)
                playerHealth.ResetViral();
        }
        result?.Invoke(res.winner == TeamId.Heroes);

        // RewardsAndPunishments(res.winner, playerObj, enemyObj);
        SendReplayToClientClientRpc(heroRoster.name, enemyRoster.name,
            res.winner.ToString(), res.duration, dto,
            new ClientRpcParams
            {
                Send = new ClientRpcSendParams { TargetClientIds = new[] { playerNet.OwnerClientId } }
            });
    }
    #region Get Persent

    private float GetHealthPercent(List<ItemData> itemDatas)
    {
        float persent = 1f;
        foreach (var itemData in itemDatas)
        {
            if (itemData is HeroData heroData && heroData.isCharacter)
            {
                if (heroData.healthPersent > 0)
                    persent = heroData.healthPersent;
            }
        }
        return persent;
    }
    private float GetManaPercent(List<ItemData> itemDatas)
    {
        float persent = 1f;
        foreach (var itemData in itemDatas)
        {
            if (itemData is HeroData heroData && heroData.isCharacter)
            {
                if (heroData.manaPersent > 0)
                    persent = heroData.manaPersent;
            }
        }
        return persent;
    }
    private float GetSpiritPercent(List<ItemData> itemDatas)
    {
        float persent = 1f;
        foreach (var itemData in itemDatas)
        {
            if (itemData is HeroData heroData && heroData.isCharacter)
            {
                if (heroData.spiritPersent > 0)
                    persent = heroData.spiritPersent;
            }
        }
        return persent;
    }

    #endregion

    private void ApplyCharacterViralRatioFromBattle(List<BattleEvent> events, PlayerVitals playerVital, TeamId winner)
    {
        var battleEvent = events.Find(x => x.type == BattleEventType.End);
        if (battleEvent == null || battleEvent is BattleEventEnd == false) return;
        var battleEventEnd = (BattleEventEnd)battleEvent;
        if (winner == TeamId.Heroes)
        {
            var healthPersent = (float)battleEventEnd.curHealthHero / battleEventEnd.maxHealthHero;
            var manaPersent = (float)battleEventEnd.curManaHero / battleEventEnd.maxManaHero;
            var spiritPersent = (float)battleEventEnd.curSpiritHero / battleEventEnd.maxSpiritHero;
            playerVital.SetViral(healthPersent, manaPersent, spiritPersent);
        }
        else
        {
            var healthPersent = battleEventEnd.curHealthEnemy / battleEventEnd.maxHealthEnemy;
            var manaPersent = battleEventEnd.curManaEnemy / battleEventEnd.maxManaEnemy;
            var spiritPersent = battleEventEnd.curSpiritEnemy / battleEventEnd.maxSpiritEnemy;
            playerVital.SetViral(healthPersent, manaPersent, spiritPersent);
        }
    }

    [ClientRpc]
    private void SendReplayToClientClientRpc(string namePlayer, string nameEnemy, string winner, float duration, BattleEventDTO[] events, ClientRpcParams rpcParams = default)
    {
        Debug.Log($"Đội chiến thắng là: {winner} với thời gian {duration} giây");
        if (winner == "Heroes")
        {
            TopNotificationUI.Instance.
                ShowNotification(
                    $"{TextColorUtil.Color("Bạn đã thắng", Color.green)}! với thời gian {TextColorUtil.Color(duration.ToString(), Color.yellow)} giây");
        }
        else
        {
            TopNotificationUI.Instance.
                ShowNotification(
                    $"{TextColorUtil.Color("Bạn đã thua", Color.red)}! với thời gian {TextColorUtil.Color(duration.ToString(), Color.yellow)} giây");
        }
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