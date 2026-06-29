using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.Netcode;
using UnityEngine;

public class LevelUpValidator : SingletonNetwork<LevelUpValidator>
{
    private LevelUpDatabase levelUpStranlation;
    public event Action<CheckLevelUpValidationResult> OnNotificationConditionResult;
    protected override void Awake()
    {
        base.Awake();
        levelUpStranlation = LevelUpDatabase.Instance;
    }

    #region Client Request To Server
    public void RequestRealmLevelUp(ulong playerNetId, string instanceId)
    {
        ValidateRealmLevelUpServerRpc(playerNetId, instanceId);
    }
    #endregion
    #region RPC Server

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void ValidateRealmLevelUpServerRpc(ulong playerClientId, string instanceId)
    {
        if (!IsServer)
            return;
        LevelUpValidationResult result = new();
        if (!NetworkManager.ConnectedClients.TryGetValue(playerClientId, out var client))
            return;
        var playerObj = client.PlayerObject;

        var playerProfile = playerObj.GetComponent<PlayerProfile>();
        var resource = playerObj.GetComponent<ResourceStorage>();
        var playerResource = playerProfile.GetPlayerResource();


        RealmData realmData = GameDataCenterManager.Instance.GetItemById(instanceId) as RealmData;


        var realmType = realmData.realmType;
        if (realmType == RealmType.PhiThang)
            result = new LevelUpValidationResult(false, "Đã đạt cấp độ tối đa, không thể lên cấp tiếp");

        var nextRealm = levelUpStranlation.GetNextRealm(realmType);
        if (nextRealm == null)
            result = new LevelUpValidationResult(false, "Không tìm thấy realm tiếp theo");

        LevelUpConditionData conditionData = new(nextRealm.itemsCost);
        conditionData.conditionType = LevelUpConditionType.ChampionLevel;
        conditionData.linhThach = nextRealm.linhThachCost;
        conditionData.requiredItem = nextRealm.itemsCost;


        if (realmData == null)
            result = new LevelUpValidationResult(false, "không hợp lệ");

        if (playerResource == null)
            result = new LevelUpValidationResult(false, "không hợp lệ");

        if (CheckResources(playerResource, conditionData) == false)
        {
            result = new LevelUpValidationResult(false, "Không đủ nguyên liệu lên cấp");
            SendMessegeToClientRpc(JsonConvert.SerializeObject(result), RpcTargetUtils.Single(playerObj.OwnerClientId));
            return;
        }

        float roll = UnityEngine.Random.value;
        float finalRate = GetFinalBreakthroughRate(playerResource, conditionData, nextRealm);
        ConsumeResources(playerResource, conditionData);
        resource.SetPlayerResource(playerResource);
        long startTime = TimeUtils.DateTimeOffset();
        long endTime = TimeUtils.DateTimeOffset(nextRealm.timeSeconds);

        bool rollResut = roll <= finalRate;

        string realmTxt = TextColorUtil.Color(EnumTranslator.ToVietnamese(nextRealm.realmType), Color.green);
        result = new LevelUpValidationResult(true, $"Đợi {TimeUtils.FormatRemainingTime(endTime)} để đột phá lên cảnh giới {realmTxt}");

        result.conditionType = LevelUpConditionType.ChampionLevel;
        result.endTime = endTime;
        result.startTime = startTime;
        result.result = rollResut;
        result.instanceId = nextRealm.instanceId;
        result.playerId = playerProfile.GetPlayerId().ToString();
        result.rewardPotentialPoint = nextRealm.rewardPotentialPoint;
        result.rewardSkillPoint = nextRealm.rewardSkillPoint;
        result.finalBreakthroughRate = finalRate;
        SegmentRealmManager.Instance.AddRealmSegment(result);
        SendMessegeToClientRpc(JsonConvert.SerializeObject(result), RpcTargetUtils.Single(playerObj.OwnerClientId));
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void RequestCheckConditionResultSerserRpc(ulong playerClientId, string instanceId)
    {
        if(IsSpawned == false)
            return;
        if (!IsServer)
            return;
        CheckLevelUpValidationResult result = new();
        result.result = true;
        if (!NetworkManager.ConnectedClients.TryGetValue(playerClientId, out var client))
            return;

        var playerObj = client.PlayerObject;
        var profile = playerObj.GetComponent<PlayerProfile>();
        if (profile == null) return;
        var playerResource = profile.GetPlayerResource();
        var realmData = GameDataCenterManager.Instance.GetItemById(instanceId) as RealmData;
        var nextRealm = levelUpStranlation.GetNextRealm(realmData.realmType);
        if (nextRealm == null)
        {
            result.message = "Đã đạt cấp độ tối đa, không thể lên cấp tiếp";
            result.result = false;
            string json = JsonConvert.SerializeObject(result);
            SendMessegeConditionToClientRpc(json, RpcTargetUtils.Single(playerClientId));
            return;
        }
        var conditionData = new LevelUpConditionData(nextRealm.itemsCost);
        conditionData.linhThach = nextRealm.linhThachCost;

        List<IResourceValidator> validators = new List<IResourceValidator>();

        if (conditionData.linhThach > 0)
            validators.Add(new LinhThachResource(conditionData.linhThach));
        if (conditionData.khoangThach > 0)
            validators.Add(new KhoangThachResource(conditionData.khoangThach));
        if (conditionData.linhThao > 0)
            validators.Add(new LinhThaoResource(conditionData.linhThao));
        if (conditionData.maHach > 0)
            validators.Add(new MaHachResource(conditionData.maHach));
        if (conditionData.yeuDan > 0)
            validators.Add(new YeuDanResource(conditionData.yeuDan));
        if (string.IsNullOrEmpty(conditionData.requiredItem) == false)
            validators.Add(new TrucCoDanReource(conditionData.requiredItem));

        foreach (var validator in validators)
        {
            if (validator.CanUse(playerResource, null) == false)
            {
                result.results.Add(new LevelUpValidationResult(false,
                $"{validator.GetResourceName()} <color=red>{validator.GetCurrentAmount(playerResource)} / {validator.GetRequiredAmount()} </color>"));
            }
            else
            {
                result.results.Add(new LevelUpValidationResult(true,
                $"{validator.GetResourceName()} <color=green>{validator.GetCurrentAmount(playerResource)} / {validator.GetRequiredAmount()} </color>"));
            }
        }
        result.finalBreakthroughRate = GetFinalBreakthroughRate(playerResource, conditionData, nextRealm);
        string message = JsonConvert.SerializeObject(result);
        SendMessegeConditionToClientRpc(message, RpcTargetUtils.Single(playerClientId));
    }
    #endregion

    private bool CheckResources(PlayerResource playerResource, LevelUpConditionData condition)
    {
        if (!IsServer)
            return false;
        List<IResourceValidator> validators = new List<IResourceValidator>();
        if (condition == null)
        {
            Debug.LogError("LevelUpConditionData is null");
            return false;
        }
        if (condition.khoangThach > 0)
            validators.Add(new KhoangThachResource(condition.khoangThach));
        if (condition.linhThach > 0)
            validators.Add(new LinhThachResource(condition.linhThach));
        if (condition.linhThao > 0)
            validators.Add(new LinhThaoResource(condition.linhThao));
        if (condition.maHach > 0)
            validators.Add(new MaHachResource(condition.maHach));
        if (condition.yeuDan > 0)
            validators.Add(new YeuDanResource(condition.yeuDan));
        if (string.IsNullOrEmpty(condition.requiredItem) == false)
            validators.Add(new TrucCoDanReource(condition.requiredItem));

        bool result = true;
        foreach (var validator in validators)
        {
            if (validator.CanUse(playerResource, null) == false)
            {
                result = false;
                break;
            }
        }
        return result;
    }

    private float GetFinalBreakthroughRate(PlayerResource playerResource, LevelUpConditionData conditionData, RealmData nextRealm)
    {
        float baseRate = nextRealm.rate;
        float bonusRate = GetBreakthroughRateBonus(playerResource, conditionData, nextRealm);
        return Mathf.Clamp01(baseRate + bonusRate);
    }

    private float GetBreakthroughRateBonus(PlayerResource playerResource, LevelUpConditionData conditionData, RealmData nextRealm)
    {
        if (playerResource == null || conditionData == null)
            return 0f;

        float totalRate = 0f;
        foreach (ItemAmount itemAmount in ResolveBreakthroughPillConsumes(playerResource, conditionData))
        {
            ItemData itemData = GameDataCenterManager.Instance.GetItemById(itemAmount.instanceId);
            if (itemData is not PillData pillData || pillData.rate <= 0f)
                continue;

            totalRate += pillData.rate * itemAmount.amount;
        }

        return Mathf.Min(totalRate, nextRealm.increaseRateMax);
    }

    private List<ItemAmount> ResolveBreakthroughPillConsumes(PlayerResource playerResource, LevelUpConditionData conditionData)
    {
        List<ItemAmount> result = new();
        if (playerResource == null || conditionData == null)
            return result;

        List<ItemAmount> sortedOwnedPills = new(playerResource.itemAmounts);
        sortedOwnedPills.Sort((a, b) =>
        {
            float rateA = GetBreakthroughPillRate(a.instanceId);
            float rateB = GetBreakthroughPillRate(b.instanceId);
            return rateB.CompareTo(rateA);
        });

        foreach (ItemAmount requiredPill in conditionData.GetBreakthroughPills())
        {
            int remaining = requiredPill.amount;
            if (remaining <= 0)
                continue;

            foreach (ItemAmount ownedPill in sortedOwnedPills)
            {
                if (ownedPill.itemId != requiredPill.itemId || ownedPill.amount <= 0)
                    continue;

                int consumeAmount = Mathf.Min(remaining, ownedPill.amount);
                result.Add(new ItemAmount(ownedPill.instanceId, ownedPill.itemId, consumeAmount));
                remaining -= consumeAmount;

                if (remaining <= 0)
                    break;
            }
        }

        return result;
    }

    private float GetBreakthroughPillRate(string instanceId)
    {
        ItemData itemData = GameDataCenterManager.Instance.GetItemById(instanceId);
        if (itemData is PillData pillData)
            return pillData.rate;

        return 0f;
    }

    private void ConsumeResources(PlayerResource playerResource, LevelUpConditionData condition)
    {
        if (!IsServer)
            return;

        new LinhThachResource().Consume(playerResource, condition.linhThach);
        new LinhThaoResource().Consume(playerResource, condition.linhThao);
        new KhoangThachResource().Consume(playerResource, condition.khoangThach);
        new MaHachResource().Consume(playerResource, condition.maHach);
        new YeuDanResource().Consume(playerResource, condition.yeuDan);
        foreach (ItemAmount itemAmount in ResolveBreakthroughPillConsumes(playerResource, condition))
        {
            ConsumeBreakthroughPill(playerResource, itemAmount);
        }
    }

    private void ConsumeBreakthroughPill(PlayerResource playerResource, ItemAmount consumedItem)
    {
        foreach (ItemAmount ownedItem in playerResource.itemAmounts)
        {
            if (ownedItem.instanceId != consumedItem.instanceId)
                continue;

            ownedItem.amount -= consumedItem.amount;
            if (ownedItem.amount < 0)
                ownedItem.amount = 0;
            return;
        }
    }
    public void RequestCheckConditionResult(ulong playerClientId, string instanceId)
    {
        RequestCheckConditionResultSerserRpc(playerClientId, instanceId);
    }

    #region RPC Client

    [ClientRpc]
    private void SendMessegeConditionToClientRpc(string message, ClientRpcParams clientRpcParams)
    {
        var results = JsonConvert.DeserializeObject<CheckLevelUpValidationResult>(message);

        if (results != null && results.result == true)
            OnNotificationConditionResult?.Invoke(results);
        else if (results != null && string.IsNullOrEmpty(results.message) == false)
            TopNotificationUI.Instance.ShowNotification(results.message);
    }
    [ClientRpc]
    private void SendMessegeToClientRpc(string message, ClientRpcParams clientRpcParams)
    {

        var messege = JsonConvert.DeserializeObject<LevelUpValidationResult>(message);
        TopNotificationUI.Instance.ShowNotification(messege.messege + "\n tỉ lệ thành công: " + messege.finalBreakthroughRate);
    }
    #endregion

}
