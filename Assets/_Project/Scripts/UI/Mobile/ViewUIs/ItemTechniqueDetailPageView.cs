using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using static LevelUpValidator;

public class ItemTechniqueDetailPageView : IItemDetailPageView
{
    [SerializeField] private UIItemConditionLevelup conditionLevelupPrefab;
    [SerializeField] private Transform contentCondition;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI techniquenameTxt;
    [SerializeField] private TextMeshProUGUI realmTxt;
    [SerializeField] private TextMeshProUGUI effectDescriptionTxt;
    [SerializeField] private TextMeshProUGUI descriptionTxt;
    [Space]
    [SerializeField] private Image nextItemIcon;
    [SerializeField] private TextMeshProUGUI nextTechniquenameTxt;
    [SerializeField] private TextMeshProUGUI nextRealmTxt;
    [SerializeField] private TextMeshProUGUI nextEffectDescriptionTxt;
    [SerializeField] private TextMeshProUGUI nextDescriptionTxt;
    [SerializeField] private Button levelUpBtn;
    [SerializeField] private TechniqueData itemData;
    [SerializeField] private TechniqueData nextItemData;
    private LevelUpValidator levelUpValidator;
    private LevelUpDatabase levelUpDatabase;
    private ulong playerClientId;
    protected override void Awake()
    {
        base.Awake();
        levelUpValidator = LevelUpValidator.Instance;
        levelUpDatabase = LevelUpDatabase.Instance;
        playerClientId = NetworkManager.Singleton.LocalClientId;

        levelUpBtn.onClick.AddListener(OnLevelUpButtonClicked);
        levelUpValidator.OnNotificationConditionResult += OnNotificationConditionResult;

        if (itemData != null)
        {
            var conditionData = new LevelUpConditionData();
            conditionData.conditionType = LevelUpConditionType.TechniqueLevel;
            conditionData.linhThach = itemData.linhThachCost;
            conditionData.linhThao = itemData.linhThaoCost;
            conditionData.khoangThach = itemData.khoangThachCost;
            conditionData.yeuDan = itemData.yeuDanCost;
            conditionData.maHach = itemData.maHachCost;
            levelUpValidator.RequestCheckConditionResult(playerClientId, conditionData);
        }
    }

    private void OnNotificationConditionResult(LevelUpValidator.CheckLevelUpValidationResult notifications)
    {
        if (notifications == null) return;
        RemoveAllNotification();
        foreach (var noti in notifications.results)
        {
            var uiCondition = Instantiate(conditionLevelupPrefab, contentCondition);
            uiCondition.Setup(noti.Message, noti.IsValid);
        }
    }
    public void RemoveAllNotification()
    {
        for (int i = 0; i < contentCondition.childCount; i++)
        {
            Destroy(contentCondition.GetChild(i).gameObject);
        }
    }
    private void OnLevelUpButtonClicked()
    {
        if (!NetworkManager.Singleton.IsConnectedClient) return;
        if (levelUpValidator == null) return;
        if (itemData == null) return;
        if (nextItemData == null)
        {
            TopNotificationUI.Instance.ShowNotification("Đã đạt cấp độ tối đa");
            return;
        }
        if (itemData is TechniqueData techniqueData)
        {
            ulong PlayerNetId = NetworkManager.Singleton.LocalClientId;
            levelUpValidator.RequestTechniqueEnhance(techniqueData.instanceId, techniqueData.itemId, PlayerNetId);
        }
    }

    public override void HandleItemClicked(InventoryItem inventoryItem)
    {
        itemData = inventoryItem.data as TechniqueData;
        if (itemData == null) return;

        itemIcon.sprite = itemData.itemIcon;
        techniquenameTxt.text = itemData.itemName;
        realmTxt.text = EnumTranslator.ToVietnamese(itemData.realmType);
        descriptionTxt.text = GetDescriptionText(itemData);
        effectDescriptionTxt.text = itemData.specialEffect;
        if (levelUpDatabase == null)
            levelUpDatabase = LevelUpDatabase.Instance;
        if (levelUpValidator != null)
        {
            var conditionData = new LevelUpConditionData();
            conditionData.conditionType = LevelUpConditionType.TechniqueLevel;
            conditionData.linhThach = itemData.linhThachCost;
            conditionData.linhThao = itemData.linhThaoCost;
            conditionData.khoangThach = itemData.khoangThachCost;
            conditionData.yeuDan = itemData.yeuDanCost;
            conditionData.maHach = itemData.maHachCost;
            levelUpValidator.RequestCheckConditionResult(playerClientId, conditionData);
        }

        nextItemData = levelUpDatabase.GetNextTechniqueEnhance(itemData.instanceId, itemData.enhanceLevel);

        if (nextItemData != null)
        {
            nextItemIcon.gameObject.SetActive(true);
            nextItemIcon.sprite = nextItemData.itemIcon;
            nextTechniquenameTxt.text = nextItemData.itemName;
            nextRealmTxt.text = nextItemData.realm + "";
            nextEffectDescriptionTxt.text = nextItemData.specialEffect;
            nextDescriptionTxt.text = GetDescriptionText(nextItemData);
        }
        else
        {
            nextItemIcon.gameObject.SetActive(false);
            nextTechniquenameTxt.text = "";
            nextRealmTxt.text = "";
            nextEffectDescriptionTxt.text = "";
            nextDescriptionTxt.text = "";
        }
    }
    public string GetDescriptionText(TechniqueData technique)
    {
        string description = "";

        // Base Damage Stats (from ItemData)
        if (technique.physicalDamage > 0)
            description += $"+ Sát thương linh thức: {technique.physicalDamage}\n";
        if (technique.magicalDamage > 0)
            description += $"+ Sát thương linh vật: {technique.magicalDamage}\n";
        if (technique.spiritDamage > 0)
            description += $"+ Sát thương linh lực: {technique.spiritDamage}\n";

        // Base Defense Stats (from ItemData)
        if (technique.physicalDefense > 0)
            description += $"+ Phòng thủ linh thức: {technique.physicalDefense}\n";
        if (technique.magicalDefense > 0)
            description += $"+ Phòng thủ linh vật: {technique.magicalDefense}\n";
        if (technique.spiritDefense > 0)
            description += $"+ Phòng thủ linh lực: {technique.spiritDefense}\n";

        // Resource Cost
        if (technique.healthCost > 0)
            description += $"+ Chi phí Sinh lực: {technique.healthCost}\n";
        if (technique.manaCost > 0)
            description += $"+ Chi phí Linh lực: {technique.manaCost}\n";
        if (technique.spiritCost > 0)
            description += $"+ Chi phí Linh thức: {technique.spiritCost}\n";

        // Combat Behavior
        if (technique.attackRange > 0)
            description += $"+ Phạm vi tấn công: {technique.attackRange}\n";
        if (technique.cooldown > 0)
            description += $"+ Thời gian hồi chiêu: {technique.cooldown}\n";
        if (technique.attackSpeed > 0)
            description += $"+ Tốc độ tấn công: {technique.attackSpeed}\n";

        // Offensive Stats Bonus
        if (technique.critDamage > 0)
            description += $"+ Sát thương chí mạng: {technique.critDamage}\n";
        if (technique.critRate > 0)
            description += $"+ Tỷ lệ chí mạng: {technique.critRate}\n";
        if (technique.armorPenetration > 0)
            description += $"+ Xuyên phá giáp: {technique.armorPenetration}\n";
        if (technique.trueDamage > 0)
            description += $"+ Sát thương thực: {technique.trueDamage}\n";
        if (technique.lifeSteal > 0)
            description += $"+ Hút máu: {technique.lifeSteal}\n";

        // Defensive Stats Bonus
        if (technique.penetrationReduction > 0)
            description += $"+ Giảm xuyên phá: {technique.penetrationReduction}\n";
        if (technique.critDamageReduction > 0)
            description += $"+ Giảm sát thương chí mạng: {technique.critDamageReduction}\n";
        if (technique.trueDamageReduction > 0)
            description += $"+ Giảm sát thương thực: {technique.trueDamageReduction}\n";

        // Resource Bonus
        if (technique.bonusHealth > 0)
            description += $"+ Tăng Sinh lực: {technique.bonusHealth}\n";
        if (technique.bonusMana > 0)
            description += $"+ Tăng Linh lực: {technique.bonusMana}\n";
        if (technique.bonusSpirit > 0)
            description += $"+ Tăng Linh thức: {technique.bonusSpirit}\n"; ;

        // Remove trailing newline if exists
        if (description.EndsWith("\n"))
            description = description.Substring(0, description.Length - 1);

        return description;
    }


}
