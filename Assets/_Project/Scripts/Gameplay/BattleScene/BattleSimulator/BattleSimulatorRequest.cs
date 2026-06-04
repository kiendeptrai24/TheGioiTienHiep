

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
            var snap = SnapshotMapper.FromStats(stats, TeamId.Heroes, heroHealthPersent);
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

            var snap = SnapshotMapper.FromStats(stats, TeamId.Enemies, enemyHealthPersent);

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
        var playerHealth = playerNet.gameObject.GetComponent<PlayerHealth>();
        var enemyHealth = enemyNO.gameObject.GetComponent<PlayerHealth>();
        if (res.winner == TeamId.Heroes)
        {
            if (playerNet.IsPlayerObject)
            {
                if (playerHealth != null)
                {
                    ApplyCharacterHealthRatioFromBattle(heroRoster.itemDatas, res.events, playerHealth);
                }
            }
            if (enemyNO.IsPlayerObject)
                enemyHealth.ResetHealth();
        }
        else
        {
            if (enemyNO.IsPlayerObject)
            {
                if (enemyHealth != null)
                {
                    ApplyCharacterHealthRatioFromBattle(enemyRoster.itemDatas, res.events, enemyHealth);
                }
            }
            if (playerNet.IsPlayerObject)
                playerHealth.ResetHealth();
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

    private void ApplyCharacterHealthRatioFromBattle(List<ItemData> itemDatas, List<BattleEvent> events, PlayerHealth playerHealth)
    {
        if (itemDatas == null || events == null || playerHealth == null) return;

        // Find character UID and HeroData
        string characterUid = null;
        HeroData playerCharacter = null;
        foreach (var itemData in itemDatas)
        {
            if (itemData is HeroData heroData && heroData.isCharacter)
            {
                characterUid = heroData.instanceId;
                playerCharacter = heroData;
                break;
            }
        }

        if (characterUid == null || playerCharacter == null) return;

        // Find character's max HP from BattleEventInit
        int characterMaxHp = -1;
        int characterInitialHp = -1;

        foreach (var ev in events)
        {
            if (ev is BattleEventInit initEv && initEv.ownerUid == characterUid)
            {
                characterMaxHp = initEv.maxHp;
                characterInitialHp = initEv.curtHp;
                break;
            }
        }

        if (characterMaxHp <= 0) return;

        // Find final HP from attack/skill events
        int finalHp = characterInitialHp;
        foreach (var ev in events)
        {
            if (ev is BattleEventDealth deathEv && deathEv.targetUid == characterUid)
            {
                finalHp = 0;
                break;
            }

            if (ev is BattleEventAttack atkEv && atkEv.targetUid == characterUid)
            {
                finalHp = atkEv.targetHpAfter;
            }
            else if (ev is BattleEventSkill skillEv && skillEv.targetUid == characterUid)
            {
                finalHp = skillEv.targetHpAfter;
            }
        }

        // Calculate health ratio from battle result
        float healthRatio = Mathf.Clamp01((float)finalHp / characterMaxHp);

        // Get player's character current max health (after all stat modifiers)
        stats.SetUp(playerCharacter);
        int playerMaxHp = stats.Health;

        if (playerMaxHp <= 0) return;

        // Apply ratio to player's character
        int newCurrentHealth = Mathf.RoundToInt(playerMaxHp * healthRatio);
        if (playerHealth.GetCurHealth() <= newCurrentHealth)
        {
            playerHealth.ResetHealth();
        }
        else
        {
            playerHealth.SetCurrentHealth(newCurrentHealth);
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