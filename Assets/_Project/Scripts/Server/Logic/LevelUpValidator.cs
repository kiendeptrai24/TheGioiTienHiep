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
        ConsumeResources(playerResource, conditionData);
        resource.SetPlayerResource(playerResource);
        long startTime = TimeUtils.DateTimeOffset();
        long endTime = TimeUtils.DateTimeOffset(nextRealm.timeSeconds);

        bool rollResut = roll <= nextRealm.rate;

        string realmTxt = TextColorUtil.Color(EnumTranslator.ToVietnamese(nextRealm.realmType), Color.green);
        result = new LevelUpValidationResult(true, $"Đợi {TimeUtils.FormatRemainingTime(endTime)} để đột phá lên cảnh giới {realmTxt}");

        playerProfile.SetPotentialPoint(nextRealm.rewardPotentialPoint);
        playerProfile.SetSkillPoint(nextRealm.rewardSkillPoint);

        result.conditionType = LevelUpConditionType.ChampionLevel;
        result.endTime = endTime;
        result.startTime = startTime;
        result.result = rollResut;
        result.instanceId = nextRealm.instanceId;
        result.playerId = playerProfile.GetPlayerId().ToString();

        SegmentRealmManager.Instance.AddRealmSegment(result);
        SendMessegeToClientRpc(JsonConvert.SerializeObject(result), RpcTargetUtils.Single(playerObj.OwnerClientId));
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void RequestCheckConditionResultSerserRpc(ulong playerClientId, string instanceId)
    {
        if (!IsServer)
            return;
        CheckLevelUpValidationResult levelupChecking = new();

        if (!NetworkManager.ConnectedClients.TryGetValue(playerClientId, out var client))
            return;

        var playerObj = client.PlayerObject;
        var profile = playerObj.GetComponent<PlayerProfile>();
        if (profile == null) return;
        var playerResource = profile.GetPlayerResource();
        var realmData = GameDataCenterManager.Instance.GetItemById(instanceId) as RealmData;
        var nextRealm = levelUpStranlation.GetNextRealm(realmData.realmType);

        var conditionData = new LevelUpConditionData(nextRealm.itemsCost);
        if (nextRealm != null)
        {
            conditionData.linhThach = nextRealm.linhThachCost;
        }

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
                levelupChecking.results.Add(new LevelUpValidationResult(false,
                $"{validator.GetResourceName()} <color=red>{validator.GetCurrentAmount(playerResource)} / {validator.GetRequiredAmount()} </color>"));
            }
            else
            {
                levelupChecking.results.Add(new LevelUpValidationResult(true,
                $"{validator.GetResourceName()} <color=green>{validator.GetCurrentAmount(playerResource)} / {validator.GetRequiredAmount()} </color>"));
            }
        }

        string message = JsonConvert.SerializeObject(levelupChecking);
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
        validators.Add(new KhoangThachResource(condition.khoangThach));
        validators.Add(new LinhThachResource(condition.linhThach));
        validators.Add(new LinhThaoResource(condition.linhThao));
        validators.Add(new MaHachResource(condition.maHach));
        validators.Add(new YeuDanResource(condition.yeuDan));
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
    private void ConsumeResources(PlayerResource playerResource, LevelUpConditionData condition)
    {
        if (!IsServer)
            return;

        new LinhThachResource().Consume(playerResource, condition.linhThach);
        new LinhThaoResource().Consume(playerResource, condition.linhThao);
        new KhoangThachResource().Consume(playerResource, condition.khoangThach);
        new MaHachResource().Consume(playerResource, condition.maHach);
        new YeuDanResource().Consume(playerResource, condition.yeuDan);
        new TrucCoDanReource().Consume(playerResource, condition.GetTrucCoDan());
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
        if (results != null)
            OnNotificationConditionResult?.Invoke(results);
    }
    [ClientRpc]
    private void SendMessegeToClientRpc(string message, ClientRpcParams clientRpcParams)
    {
        var messege = JsonConvert.DeserializeObject<LevelUpValidationResult>(message);
        TopNotificationUI.Instance.ShowNotification(messege.message);
    }
    #endregion

}
