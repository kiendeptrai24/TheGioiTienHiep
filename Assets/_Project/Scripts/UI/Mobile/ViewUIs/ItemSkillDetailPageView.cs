using System;
using TGTH.Mobile;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using static LevelUpValidator;

public class ItemSkillDetailPageView : IItemDetailPageView
{
    [SerializeField] private UIItemConditionLevelup conditionLevelupPrefab;
    [SerializeField] private Transform contentCondition;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI skillnameTxt;
    [SerializeField] private TextMeshProUGUI realmTxt;
    [SerializeField] private TextMeshProUGUI effectDescriptionTxt;
    [SerializeField] private TextMeshProUGUI descriptionTxt;
    [Space]
    [SerializeField] private Image nextItemIcon;
    [SerializeField] private TextMeshProUGUI nextSkillnameTxt;
    [SerializeField] private TextMeshProUGUI nextRealmTxt;
    [SerializeField] private TextMeshProUGUI nextEffectDescriptionTxt;
    [SerializeField] private TextMeshProUGUI nextDescriptionTxt;
    [SerializeField] private Button levelUpBtn;
    [SerializeField] private SkillData itemData;
    [SerializeField] private SkillData nextItemData;
    private LevelUpDatabase levelUpDatabase;
    private ulong playerClientId;
    private LevelUpValidator levelUpValidator;
    private InventoryCenterManager inventoryCenterManager;
    protected override void Awake()
    {
        base.Awake();
        levelUpValidator = LevelUpValidator.Instance;
        levelUpDatabase = LevelUpDatabase.Instance;
        inventoryCenterManager = InventoryCenterManager.Instance;
        playerClientId = NetworkManager.Singleton.LocalClientId;

        levelUpBtn.onClick.AddListener(OnLevelUpButtonClicked);
        levelUpValidator.OnNotificationConditionResult += OnNotificationConditionResult;
        inventoryCenterManager.OnItemUpdated += OnItemUpdated;
        if (itemData != null)
        {
            levelUpValidator.RequestCheckConditionResult(playerClientId, itemData.instanceId);
        }
    }

    private void OnItemUpdated(ItemData data, string instanceIdOld)
    {
        if (data == null || itemData == null) return;
        if (itemData.instanceId == instanceIdOld)
        {
            var inventoryItem = new InventoryItem(data);
            HandleItemClicked(inventoryItem);
        }
        else
        {
            Debug.Log("dsaaa");
        }
    }

    private void OnNotificationConditionResult(CheckLevelUpValidationResult notifications)
    {
        if (notifications == null) return;
        RemoveAllNotification();
        foreach (var noti in notifications.results)
        {
            var uiCondition = Instantiate(conditionLevelupPrefab, contentCondition);
            uiCondition.Setup(noti.messege, noti.result);
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
        if (levelUpDatabase == null) return;
        if (itemData == null) return;
        if (nextItemData == null)
        {
            TopNotificationUI.Instance.ShowNotification("Đã đạt cấp độ tối đa");
            return;
        }
        if (itemData is SkillData skillData)
        {
            ulong PlayerNetId = NetworkManager.Singleton.LocalClientId;
        }
    }
    public override void HandleItemClicked(InventoryItem inventoryItem)
    {
        itemData = inventoryItem.data as SkillData;
        if (itemData == null) return;

        itemIcon.sprite = itemData.itemIcon;
        skillnameTxt.text = itemData.itemName;
        realmTxt.text = EnumTranslator.ToVietnamese(itemData.realmType);
        effectDescriptionTxt.text = itemData.specialEffect;
        descriptionTxt.text = GetDescriptionText(itemData);

        if (levelUpDatabase == null)
            levelUpDatabase = LevelUpDatabase.Instance;
        if (levelUpValidator != null)
        {
            levelUpValidator.RequestCheckConditionResult(playerClientId, itemData.instanceId);
        }
        nextItemData = levelUpDatabase.GetNextSkillEnhance(itemData.instanceId, itemData.enhanceLevel);

        if (nextItemData != null)
        {
            nextItemIcon.gameObject.SetActive(true);
            nextItemIcon.sprite = nextItemData.itemIcon;
            nextSkillnameTxt.text = nextItemData.itemName;
            nextRealmTxt.text = nextItemData.realmType + "";
            nextEffectDescriptionTxt.text = nextItemData.specialEffect;
            nextDescriptionTxt.text = GetDescriptionText(nextItemData);
        }
        else
        {
            nextItemIcon.gameObject.SetActive(false);
            nextSkillnameTxt.text = "";
            nextRealmTxt.text = "";
            nextEffectDescriptionTxt.text = "";
            nextDescriptionTxt.text = "";
        }
    }
    public string GetDescriptionText(SkillData skill)
    {
        string description = "";

        // Base Damage Stats (from ItemData)
        if (skill.physicalDamage > 0)
            description += $"+ Sát thương linh thức: {skill.physicalDamage}\n";
        if (skill.magicalDamage > 0)
            description += $"+ Sát thương linh vật: {skill.magicalDamage}\n";
        if (skill.spiritDamage > 0)
            description += $"+ Sát thương linh lực: {skill.spiritDamage}\n";

        // Base Defense Stats (from ItemData)
        if (skill.physicalDefense > 0)
            description += $"+ Phòng thủ linh thức: {skill.physicalDefense}\n";
        if (skill.magicalDefense > 0)
            description += $"+ Phòng thủ linh vật: {skill.magicalDefense}\n";
        if (skill.spiritDefense > 0)
            description += $"+ Phòng thủ linh lực: {skill.spiritDefense}\n";

        // Resource Cost
        if (skill.healthCost > 0)
            description += $"+ Chi phí Sinh lực: {skill.healthCost}\n";
        if (skill.manaCost > 0)
            description += $"+ Chi phí Linh lực: {skill.manaCost}\n";
        if (skill.spiritCost > 0)
            description += $"+ Chi phí Linh thức: {skill.spiritCost}\n";

        // Combat Behavior
        if (skill.attackRange > 0)
            description += $"+ Phạm vi tấn công: {skill.attackRange}\n";
        if (skill.cooldown > 0)
            description += $"+ Thời gian hồi chiêu: {skill.cooldown}\n";
        if (skill.attackSpeed > 0)
            description += $"+ Tốc độ tấn công: {skill.attackSpeed}\n";

        // Offensive Stats Bonus
        if (skill.critDamage > 0)
            description += $"+ Sát thương chí mạng: {skill.critDamage}\n";
        if (skill.critRate > 0)
            description += $"+ Tỷ lệ chí mạng: {skill.critRate}\n";
        if (skill.armorPenetration > 0)
            description += $"+ Xuyên phá giáp: {skill.armorPenetration}\n";
        if (skill.trueDamage > 0)
            description += $"+ Sát thương thực: {skill.trueDamage}\n";
        if (skill.lifeSteal > 0)
            description += $"+ Hút máu: {skill.lifeSteal}\n";

        // Defensive Stats Bonus
        if (skill.penetrationReduction > 0)
            description += $"+ Giảm xuyên phá: {skill.penetrationReduction}\n";
        if (skill.critDamageReduction > 0)
            description += $"+ Giảm sát thương chí mạng: {skill.critDamageReduction}\n";
        if (skill.trueDamageReduction > 0)
            description += $"+ Giảm sát thương thực: {skill.trueDamageReduction}\n";

        // Resource Bonus
        if (skill.bonusHealth > 0)
            description += $"+ Tăng Sinh lực: {skill.bonusHealth}\n";
        if (skill.bonusMana > 0)
            description += $"+ Tăng Linh lực: {skill.bonusMana}\n";
        if (skill.bonusSpirit > 0)
            description += $"+ Tăng Linh thức: {skill.bonusSpirit}\n"; ;

        // Remove trailing newline if exists
        if (description.EndsWith("\n"))
            description = description.Substring(0, description.Length - 1);

        return description;
    }
}
