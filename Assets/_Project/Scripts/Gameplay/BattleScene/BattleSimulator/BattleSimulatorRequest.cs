

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
    private SafeZoneManager safeZoneManager;
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
    protected override void Start()
    {
        base.Start();
        if (!IsServer) return;
        safeZoneManager = SafeZoneManager.Instance;
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
        if (!NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(monsterNetId, out var player2Net))
            return;
        if (playerNet == null || player2Net == null) return;
        if (playerNet.IsPlayerObject && player2Net.IsPlayerObject)
        {
            if (safeZoneManager != null)
            {
                if (safeZoneManager.OutSide(playerNet.transform.position) || safeZoneManager.OutSide(player2Net.transform.position))
                {
                    return;
                }
            }
        }

        var playerObj = playerNet.gameObject;
        var player2Obj = player2Net.gameObject;

        if (playerObj == null || player2Obj == null) return;

        var senderNet = playerObj.GetComponent<NetworkBehaviour>();

        if (senderNet == null) return;

        ulong senderClientId = senderNet.OwnerClientId;

        if (playerObj == null) return;
        if (player2Obj == null) return;

        var playerRoster = playerObj.GetComponent<PlayerBattleRoster>();
        var player2Roster = player2Obj.GetComponent<PlayerBattleRoster>();

        float playerHealthPersent = GetHealthPercent(playerRoster.itemDatas);
        float player2HealthPersent = GetHealthPercent(player2Roster.itemDatas);

        float playerManaPersent = GetManaPercent(playerRoster.itemDatas);
        float player2ManaPersent = GetManaPercent(player2Roster.itemDatas);

        float playerSpiritPersent = GetSpiritPercent(playerRoster.itemDatas);
        float player2SpiritPersent = GetSpiritPercent(player2Roster.itemDatas);

        List<UnitInput> player2Snaps = new();
        List<UnitInput> playerSnaps = new();
        Board board = new Board
        {
            width = 5,
            height = 9,
            moveInterval = 1f,
            allowDiagonal = true
        };

        BattleBoardGrid boardGrid = new BattleBoardGrid(board.moveInterval, board.allowDiagonal);
        // HERO
        foreach (var itemData in playerRoster.itemDatas)
        {
            if (itemData == null) continue;
            stats.SetUp(itemData);
            var snap = SnapshotMapper.FromStats(stats, TeamId.Heroes, playerHealthPersent, playerManaPersent, playerSpiritPersent);
            Vector2Int pos = (stats.heroData as HeroData).championIndex;
            pos = boardGrid.ClampToValidCell(pos);

            snap.placement.cell = pos;
            snap.placement.attackRange = (int)snap.snap.attackRange;
            playerSnaps.Add(snap);
        }

        // ENEMY
        foreach (var itemData in player2Roster.itemDatas)
        {
            if (itemData == null) continue;
            stats.SetUp(itemData);

            var snap = SnapshotMapper.FromStats(stats, TeamId.Enemies, player2HealthPersent, player2ManaPersent, player2SpiritPersent);

            Vector2Int pos = (stats.heroData as HeroData).championIndex;

            pos.x = board.width - 1 - pos.x;
            pos.y = board.height - 1 - pos.y;
            pos = boardGrid.ClampToValidCell(pos);

            snap.placement.cell = pos;
            snap.placement.attackRange = (int)snap.snap.attackRange;
            player2Snaps.Add(snap);
        }

        uint seed = (uint)(playerNetId.GetHashCode() ^ monsterNetId.GetHashCode() ^ Environment.TickCount);


        var res = BattleSimulator.Simulate(playerSnaps, player2Snaps, seed, boardGrid, 60f);
        // convert to DTO
        var dto = new BattleEventDTO[res.events.Count];
        for (int i = 0; i < res.events.Count; i++)
        {
            var ev = res.events[i];
            dto[i] = BattleEventMapper.ToDTO(ev);
        }
        var playerHealth = playerNet.gameObject.GetComponent<PlayerVitals>();
        var enemyHealth = player2Net.gameObject.GetComponent<PlayerVitals>();
        if (res.winner == TeamId.Heroes)
        {
            if (playerNet.IsPlayerObject)
            {
                if (playerHealth != null)
                {
                    ApplyCharacterViralRatioFromBattle(res.events, playerHealth, res.winner);
                }
            }
            if (player2Net.IsPlayerObject)
                enemyHealth.ResetViral();
        }
        else
        {
            if (player2Net.IsPlayerObject)
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

        SendReplayToClientClientRpc(playerRoster.name, player2Roster.name,
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
    private float GetPercent(int current, int max)
    {
        if (max <= 0)
            return 0f;

        return Mathf.Clamp01((float)current / max);
    }
    private void ApplyCharacterViralRatioFromBattle(List<BattleEvent> events, PlayerVitals playerVital, TeamId winner)
    {
        var battleEvent = events.Find(x => x.type == BattleEventType.End);

        if (battleEvent is not BattleEventEnd battleEventEnd)
            return;

        if (playerVital == null)
            return;

        if (winner == TeamId.Heroes)
        {
            float healthPercent = GetPercent(
                battleEventEnd.curHealthHero,
                battleEventEnd.maxHealthHero
            );

            float manaPercent = GetPercent(
                battleEventEnd.curManaHero,
                battleEventEnd.maxManaHero
            );

            float spiritPercent = GetPercent(
                battleEventEnd.curSpiritHero,
                battleEventEnd.maxSpiritHero
            );
            if (healthPercent <= 0f)
            {
                healthPercent = 1;
                manaPercent = 1;
                spiritPercent = 1;
            }
            playerVital.SetViral(healthPercent, manaPercent, spiritPercent);
        }
        else
        {
            float healthPercent = GetPercent(
                battleEventEnd.curHealthEnemy,
                battleEventEnd.maxHealthEnemy
            );

            float manaPercent = GetPercent(
                battleEventEnd.curManaEnemy,
                battleEventEnd.maxManaEnemy
            );

            float spiritPercent = GetPercent(
                battleEventEnd.curSpiritEnemy,
                battleEventEnd.maxSpiritEnemy
            );
            if (healthPercent <= 0f)
            {
                healthPercent = 1;
                manaPercent = 1;
                spiritPercent = 1;
            }
            playerVital.SetViral(healthPercent, manaPercent, spiritPercent);
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
                            $"skillId={s.skillId} hpCost={s.healthCost} manaCost={s.manaCost} spiritCost={s.spiritCost} dmg={s.damage} crit={s.isCrit} hpAfter={s.targetHpAfter}\n\n";
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