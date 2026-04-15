using System;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Newtonsoft.Json;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Server-side logic kiểm tra điều kiện lên cấp và trả về reward items
/// </summary>
public class LevelUpValidator : SingletonNetwork<LevelUpValidator>
{
    public class LevelUpValidationResult
    {
        public bool IsValid;
        public string instanceId;
        public string itemId;
        public string Message;
        public int Level;
        public LevelUpConditionType conditionType;
        public LevelUpValidationResult() { }
        public LevelUpValidationResult(bool valid, string msg)
        {
            IsValid = valid;
            Message = msg;
        }
    }
    public class CheckLevelUpValidationResult
    {
        public List<LevelUpValidationResult> results = new();
    }
    public enum LevelUpConditionType
    {
        ChampionLevel,
        SkillLevel,
        TechniqueLevel
    }
    private LevelUpDatabase levelUpStranlation;
    public event Action<CheckLevelUpValidationResult> OnNotificationConditionResult;
    protected override void Awake()
    {
        base.Awake();
        levelUpStranlation = LevelUpDatabase.Instance;

    }
    public int GetPercentSuccess(RealmType realmType)
    {
        int percent = 0;
        switch (realmType)
        {
            case RealmType.LuyenKhi_1:
            case RealmType.LuyenKhi_2:
            case RealmType.LuyenKhi_3:
            case RealmType.LuyenKhi_4:
            case RealmType.LuyenKhi_5:
            case RealmType.LuyenKhi_6:
            case RealmType.LuyenKhi_7:
            case RealmType.LuyenKhi_8:
            case RealmType.LuyenKhi_9:
                percent = 100;
                break;
            case RealmType.TrucCo_SK:
            case RealmType.TrucCo_TK:
            case RealmType.TrucCo_HK:
            case RealmType.TrucCo_DVM:
                percent = 90;
                break;

            case RealmType.KetDan_SK:
            case RealmType.KetDan_TK:
            case RealmType.KetDan_HK:
            case RealmType.KetDan_DVM:
                percent = 80;
                break;

            case RealmType.NguyenAnh_SK:
            case RealmType.NguyenAnh_TK:
            case RealmType.NguyenAnh_HK:
            case RealmType.NguyenAnh_DVM:
                percent = 70;
                break;

            case RealmType.HoaThan_SK:
            case RealmType.HoaThan_TK:
            case RealmType.HoaThan_HK:
            case RealmType.HoaThan_DVM:
                percent = 60;
                break;

            case RealmType.HopThe_SK:
            case RealmType.HopThe_TK:
            case RealmType.HopThe_HK:
            case RealmType.HopThe_DVM:
                percent = 50;
                break;

            case RealmType.DoKiep_SK:
            case RealmType.DoKiep_TK:
            case RealmType.DoKiep_HK:
            case RealmType.DoKiep_DVM:
                percent = 40;
                break;

            case RealmType.DaiThua_SK:
            case RealmType.DaiThua_TK:
            case RealmType.DaiThua_HK:
            case RealmType.DaiThua_DVM:
                percent = 30;
                break;

            case RealmType.PhiThang:
                percent = 5;
                break;
            default:
                return 0;

        }
        return percent;
    }

    #region Client Request To Server
    public void RequestRealmLevelUp(ulong playerNetId)
    {
        ValidateHeroLevelUpServerRpc(playerNetId);
        Debug.Log($"Received level up request from client {playerNetId}");
    }
    public void RequestSkillEnhance(string skillInstanceId, string skillId, ulong playerNetId)
    {
        ValidateSkillEnhanceServerRpc(skillInstanceId, skillId, playerNetId);
    }
    public void RequestTechniqueEnhance(string techniqueInstanceId, string techniqueId, ulong playerNetId)
    {
        ValidateTechniqueEnhanceServerRpc(techniqueInstanceId, techniqueId, playerNetId);
    }
    #endregion

    #region Server Logic

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void ValidateHeroLevelUpServerRpc(ulong playerClientId)
    {
        if (!IsServer)
            return;
        LevelUpValidationResult result = new();
        Debug.Log($"Received level up request from client {playerClientId}");
        if (!NetworkManager.ConnectedClients.TryGetValue(playerClientId, out var client))
            return;
        var playerObj = client.PlayerObject;

        var statsData = playerObj.GetComponent<StatsData>();
        var resource = playerObj.GetComponent<ResourceStorage>();
        var playerResource = new PlayerResource();
        var playerProfile = playerObj.GetComponent<PlayerProfile>();

        playerResource.linhThach = (int)resource.Coins.Value;

        HeroData heroData = statsData.heroData as HeroData;

        if (heroData == null)
            result = new LevelUpValidationResult(false, "Hero data không hợp lệ");

        if (heroData.levelUpConditionData == null)
            result = new LevelUpValidationResult(false, "Điều kiện lên cấp không hợp lệ");

        var realmType = heroData.realmType;
        if (realmType == RealmType.PhiThang)
            result = new LevelUpValidationResult(false, "Đã đạt cấp độ tối đa, không thể lên cấp tiếp");

        if (playerResource == null)
            result = new LevelUpValidationResult(false, "Player resource không hợp lệ");
        if (CheckResources(playerResource, heroData.levelUpConditionData) == false)
            result = new LevelUpValidationResult(false, "Không đủ nguyên liệu lên cấp");

        var nextRealm = levelUpStranlation.GetNextRealm(realmType);
        if (nextRealm == null)
            result = new LevelUpValidationResult(false, "Không tìm thấy realm tiếp theo");
        Debug.Log($"Next realm for {realmType} is: {(nextRealm != null ? nextRealm.realmType.ToString() : "null")}");
        float success = GetPercentSuccess(nextRealm.realmType);
        float roll = UnityEngine.Random.Range(1f, 100f);
        if (CheckResources(playerResource, heroData.levelUpConditionData) == false)
            result = new LevelUpValidationResult(false, "Không đủ nguyên liệu lên cấp");

        if (roll > success)
        {
            Debug.Log($"Level up failed! Rolled {roll} needed under {success}");
            result = new LevelUpValidationResult(false, $"Đột phá thất bại!");
        }
        else
        {
            result.itemId = "";
            playerProfile.SetPotentialPoint(nextRealm.rewardPotentialPoint);
            playerProfile.SetSkillPoint(nextRealm.rewardSkillPoint);
            
            result = new LevelUpValidationResult(true, $"Đột phá thành công! cảnh giới hiện tại là {EnumTranslator.ToVietnamese(nextRealm.realmType)}");
        }
        result.conditionType = LevelUpConditionType.ChampionLevel;
        SendMessegeToClientRpc(JsonConvert.SerializeObject(result),
            new ClientRpcParams
            {
                Send = new ClientRpcSendParams { TargetClientIds = new[] { playerObj.OwnerClientId } }
            });
    }
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void ValidateSkillEnhanceServerRpc(string skillInstanceId, string skillId, ulong playerClientId)
    {
        if (!IsServer)
            return;
        LevelUpValidationResult result = new();
        if (!NetworkManager.ConnectedClients.TryGetValue(playerClientId, out var client))
            return;
        var playerObj = client.PlayerObject;
        var resource = playerObj.GetComponent<ResourceStorage>();
        var playerResource = new PlayerResource();
        playerResource.linhThach = (int)resource.Coins.Value;

        var skillData = levelUpStranlation.GetItemDict(skillInstanceId) as SkillData;

        if (skillData == null)
            result = new LevelUpValidationResult(false, "Skill data không hợp lệ");

        if (!skillData.hasLearned)
            result = new LevelUpValidationResult(false, "Phải học kỹ năng trước khi cường hóa");

        if (skillData.enhanceLevel >= skillData.maxEnhanceLevel)
            result = new LevelUpValidationResult(false,
                $"Kỹ năng đã đạt cấp độ cường hóa tối đa ({skillData.maxEnhanceLevel})");

        if (CheckResources(playerResource, skillData.levelUpConditionData) == false)
            result = new LevelUpValidationResult(false, "Không đủ nguyên liệu cường hóa");

        var nextSkill = levelUpStranlation.GetNextSkillEnhance(skillInstanceId, skillData.enhanceLevel);

        if (nextSkill == null)
        {
            result = new LevelUpValidationResult(false, "Kỹ năng đã đạt cấp độ tối đa");
        }
        else
        {
            if (CheckResources(playerResource, skillData.levelUpConditionData))
            {
                result = new LevelUpValidationResult(true,
                    $"Có thể cường hóa kỹ năng từ cấp {skillData.enhanceLevel} lên {skillData.enhanceLevel + 1}");
                ConsumeResources(playerResource, skillData.levelUpConditionData);
            }
            else
            {
                result = new LevelUpValidationResult(false, "Không đủ nguyên liệu cường hóa");
            }
        }


        result.conditionType = LevelUpConditionType.SkillLevel;
        result.Level = skillData.enhanceLevel;
        result.instanceId = skillInstanceId;
        result.itemId = skillId;
        string message = JsonConvert.SerializeObject(result);
        SendMessegeToClientRpc(message,
        new ClientRpcParams
        {
            Send = new ClientRpcSendParams { TargetClientIds = new[] { playerObj.OwnerClientId } }
        });
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void ValidateTechniqueEnhanceServerRpc(string techniqueInstanceId, string techniqueId, ulong playerClientId)
    {
        if (!IsServer)
            return;
        LevelUpValidationResult result = new();
        var techniqueData = levelUpStranlation.GetItemDict(techniqueInstanceId) as TechniqueData;

        if (!NetworkManager.ConnectedClients.TryGetValue(playerClientId, out var client))
            return;

        var playerObj = client.PlayerObject;
        var resource = playerObj.GetComponent<ResourceStorage>();
        var playerResource = new PlayerResource();
        playerResource.linhThach = (int)resource.Coins.Value;

        if (techniqueData == null)
            result = new LevelUpValidationResult(false, "Technique data không hợp lệ");

        if (!techniqueData.hasLearned)
            result = new LevelUpValidationResult(false, "Phải học chiêu thức trước khi cường hóa");

        if (techniqueData.enhanceLevel >= techniqueData.maxEnhanceLevel)
            result = new LevelUpValidationResult(false,
                $"Chiêu thức đã đạt cấp độ cường hóa tối đa ({techniqueData.maxEnhanceLevel})");

        var nextTechnique = levelUpStranlation.GetNextTechniqueEnhance(techniqueInstanceId, techniqueData.enhanceLevel);
        if (nextTechnique == null)
        {
            result = new LevelUpValidationResult(false, "Công pháp đã đạt cấp độ tối đa");
        }
        else
        {
            if (CheckResources(playerResource, techniqueData.levelUpConditionData))
            {
                result = new LevelUpValidationResult(true,
                    $"Cường hóa Công Pháp thành công Công pháp hiện tại là {nextTechnique.itemName}");
                ConsumeResources(playerResource, techniqueData.levelUpConditionData);
                result.itemId = techniqueId;
                result.instanceId = techniqueInstanceId;
                result.Level = techniqueData.enhanceLevel;
                result.conditionType = LevelUpConditionType.TechniqueLevel;
            }
            else
            {
                result = new LevelUpValidationResult(false, "Không đủ nguyên liệu cường hóa");
            }
        }


        string message = JsonConvert.SerializeObject(result);

        SendMessegeToClientRpc(message,
        new ClientRpcParams
        {
            Send = new ClientRpcSendParams { TargetClientIds = new[] { playerObj.OwnerClientId } }
        });
    }
    #endregion

    [ClientRpc]
    private void SendMessegeToClientRpc(string message, ClientRpcParams clientRpcParams)
    {
        var messege = JsonConvert.DeserializeObject<LevelUpValidationResult>(message);
        Debug.Log($"Kết quả kiểm tra lên cấp: {messege.IsValid}, lời nhắn: {messege.Message} itemId: {messege.itemId}");
        TopNotificationUI.Instance.ShowNotification(messege.Message);
        var cham = InventoryCenterManager.Instance.playerCham as HeroData;
        if (messege.IsValid)
        {
            switch (messege.conditionType)
            {
                case LevelUpConditionType.SkillLevel:
                    var nextSkill = levelUpStranlation.GetNextSkillEnhance(messege.instanceId, messege.Level);
                    if (nextSkill != null)
                    {
                        var skillData = InventoryCenterManager.Instance.GetItemData(messege.itemId);
                        if (skillData == null) return;
                        InventoryCenterManager.Instance.UpdateItemData(skillData.itemId, nextSkill);
                        Debug.Log($"Cường hóa kỹ năng thành công kỹ năng hiện tại là {nextSkill.itemName}");
                    }
                    break;
                case LevelUpConditionType.TechniqueLevel:
                    // Handle technique level up logic
                    var nextTechnique = levelUpStranlation.GetNextTechniqueEnhance(messege.instanceId, messege.Level);
                    if (nextTechnique != null)
                    {
                        var techniqueData = InventoryCenterManager.Instance.GetItemData(messege.itemId);
                        if (techniqueData == null) return;
                        InventoryCenterManager.Instance.UpdateItemData(techniqueData.itemId, nextTechnique);
                        Debug.Log($"Cường hóa Công Pháp thành công Công pháp hiện tại là {nextTechnique.itemName}");
                    }
                    break;
                case LevelUpConditionType.ChampionLevel:
                    // Handle champion level up logic
                    var nextRealm = levelUpStranlation.GetNextRealm(cham.realmType);
                    if (nextRealm != null)
                    {
                        cham.realmType = nextRealm.realmType;
                        cham.realmData = nextRealm;
                        InventoryCenterManager.Instance.ItemPlayerChanged(cham);
                    }
                    break;
            }
        }


    }
    [ContextMenu("Test Validate Hero Level Up")]
    public void Request()
    {
        RequestRealmLevelUp(NetworkManager.Singleton.LocalClientId);
    }
    private bool CheckResources(PlayerResource playerResource, LevelUpConditionData condition)
    {
        if (!IsServer)
            return false;
        Debug.Log($"linh thach: {playerResource.linhThach}, required: {condition.linhThach}");
        List<IResourceValidator> validators = new List<IResourceValidator>();
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
    }
    public void RequestCheckConditionResult(ulong playerClientId, LevelUpConditionData conditionData)
    {
        string data = JsonConvert.SerializeObject(conditionData);
        RequestCheckConditionResultSerserRpc(playerClientId, data);
    }
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void RequestCheckConditionResultSerserRpc(ulong playerClientId, string data)
    {
        if (!IsServer)
            return;
        CheckLevelUpValidationResult levelupChecking = new();
        var conditionData = JsonConvert.DeserializeObject<LevelUpConditionData>(data);
        if (conditionData == null)
            return;
        if (!NetworkManager.ConnectedClients.TryGetValue(playerClientId, out var client))
            return;

        var playerObj = client.PlayerObject;
        var profile = playerObj.GetComponent<PlayerProfile>();
        var playerResource = profile.GetPlayerResource();
        if (playerResource == null)
        {
            return;
        }

        List<IResourceValidator> validators = new List<IResourceValidator>();
        validators.Add(new KhoangThachResource(conditionData.khoangThach));
        validators.Add(new LinhThachResource(conditionData.linhThach));
        validators.Add(new LinhThaoResource(conditionData.linhThao));
        validators.Add(new MaHachResource(conditionData.maHach));
        validators.Add(new YeuDanResource(conditionData.yeuDan));

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
        SendMessegeConditionToClientRpc(message,
        new ClientRpcParams
        {
            Send = new ClientRpcSendParams { TargetClientIds = new[] { playerObj.OwnerClientId } }
        });
    }
    [ClientRpc]
    private void SendMessegeConditionToClientRpc(string message, ClientRpcParams clientRpcParams)
    {
        var results = JsonConvert.DeserializeObject<CheckLevelUpValidationResult>(message);
        if (results != null)
            OnNotificationConditionResult?.Invoke(results);
    }
}
