using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using static LevelUpValidator;

public class ItemChamDetailPageView : IItemDetailPageView
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
    [SerializeField] private RealmData itemData;
    [SerializeField] private RealmData nextItemData;
    private LevelUpValidator levelUpValidator;
    private LevelUpDatabase levelUpDatabase;
    private ulong playerClientId;
    private InventoryCenterManager inventoryCenterManager;
    private HeroData heroData;
    private bool isUpgrading = false;
    public bool canLevelup = false;

    protected override void Awake()
    {
        base.Awake();
        SetUpValidator();
        SetUpDataBase();
        SegmentRealmManager.Instance.OnRealmUplevelResult += OnRealmUplevelResult;
        SegmentRealmManager.Instance.OnRealmUpgrade += OnRealmUpgrade;
        inventoryCenterManager = InventoryCenterManager.Instance;
        levelUpBtn.onClick.AddListener(OnLevelUpButtonClicked);

        inventoryCenterManager.OnItemUpdated += OnItemUpdated;
        inventoryCenterManager.OnItemDataChanged += OnItemDataChanged;
        if (itemData != null)
        {
            OnRealmUplevelResult(true);
        }
    }
    private void OnEnable() {
        isUpgrading = SegmentRealmManager.Instance.GetIsUpdating();
    }
    private void OnItemDataChanged(List<ItemData> list)
    {
        levelUpValidator.RequestCheckConditionResult(playerClientId, itemData.instanceId);
    }

    private void OnRealmUpgrade(UpgradeState state)
    {
        isUpgrading = SegmentRealmManager.Instance.GetIsUpdating();
    }

    private void OnRealmUplevelResult(bool success)
    {
        if (!success) return;
        if (itemData == null) return;
        if (levelUpValidator == null) return;

        isUpgrading = SegmentRealmManager.Instance.GetIsUpdating();
        levelUpValidator.RequestCheckConditionResult(playerClientId, itemData.instanceId);
    }
    private void SetUpValidator()
    {
        if (levelUpValidator == null)
        {
            playerClientId = NetworkManager.Singleton.LocalClientId;
            levelUpValidator = LevelUpValidator.Instance;
            levelUpValidator.OnNotificationConditionResult += OnNotificationConditionResult;
        }
    }
    public void SetUpDataBase()
    {
        if (levelUpDatabase == null)
        {
            levelUpDatabase = LevelUpDatabase.Instance;
        }
    }
    private void OnItemUpdated(ItemData data, string instanceIdOld)
    {
        if (data == null || heroData == null) return;
        if (heroData.instanceId == instanceIdOld)
        {
            var inventoryItem = new InventoryItem(data);
            HandleItemClicked(inventoryItem);
        }
    }

    private void OnNotificationConditionResult(CheckLevelUpValidationResult notifications)
    {
        if (notifications == null) return;
        RemoveAllNotification();
        canLevelup = true;
        foreach (var noti in notifications.results)
        {
            var uiCondition = Instantiate(conditionLevelupPrefab, contentCondition);
            uiCondition.Setup(noti.message, noti.result);
            if (noti.result == false)
            {
                canLevelup = false;
            }
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
        if (canLevelup == false)
        {
            TopNotificationUI.Instance.ShowNotification("Không đủ điều kiện để đột phá");
            return;
        }
        if (isUpgrading)
        {
            TopNotificationUI.Instance.ShowNotification("Đang trong quá trình đột phá");
            return;
        }
        if (!NetworkManager.Singleton.IsConnectedClient) return;
        if (levelUpValidator == null) return;
        if (itemData == null) return;
        if (nextItemData == null)
        {
            TopNotificationUI.Instance.ShowNotification("Đã đạt cấp độ tối đa");
            return;
        }
        if (itemData is RealmData)
        {
            levelUpValidator.RequestRealmLevelUp(playerClientId, itemData.realmId);
        }
    }

    public override void HandleItemClicked(InventoryItem inventoryItem)
    {
        SetUpDataBase();
        SetUpValidator();
        heroData = inventoryItem.data as HeroData;
        itemData = heroData.realmData;
        if (itemData == null) return;

        itemIcon.sprite = itemData.itemIcon;
        techniquenameTxt.text = itemData.itemName;
        realmTxt.text = EnumTranslator.ToVietnamese(itemData.realmType);
        effectDescriptionTxt.text = itemData.itemDescription.Replace(". ", ".\n");
        descriptionTxt.text = GetDescriptionText(itemData);
        if (levelUpValidator != null)
        {
            levelUpValidator.RequestCheckConditionResult(playerClientId, itemData.instanceId);
        }

        nextItemData = levelUpDatabase.GetNextRealm(itemData.realmType);

        if (nextItemData != null)
        {
            nextItemIcon.gameObject.SetActive(true);
            nextItemIcon.sprite = nextItemData.itemIcon;
            nextTechniquenameTxt.text = nextItemData.itemName;
            nextRealmTxt.text = EnumTranslator.ToVietnamese(nextItemData.realmType);
            nextEffectDescriptionTxt.text = nextItemData.itemDescription.Replace(". ", ".\n");
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
    public string GetDescriptionText(RealmData realData)
    {
        string description = "";
        if (realData.health > 0)
            description += $"+ Sinh lực: {realData.health}\n";
        if (realData.mana > 0)
            description += $"+ Linh lực: {realData.mana}\n";
        if (realData.spirit > 0)
            description += $"+ Linh thức: {realData.spirit}\n";

        // Base Damage Stats (from ItemData)
        if (realData.physicalDamage > 0)
            description += $"+ Sát thương Linh thức: {realData.physicalDamage}\n";
        if (realData.magicalDamage > 0)
            description += $"+ Sát thương linh vật: {realData.magicalDamage}\n";
        if (realData.spiritDamage > 0)
            description += $"+ Sát thương linh lực: {realData.spiritDamage}\n";

        // Base Defense Stats (from ItemData)
        if (realData.physicalDefense > 0)
            description += $"+ Phòng thủ linh thức: {realData.physicalDefense}\n";
        if (realData.magicalDefense > 0)
            description += $"+ Phòng thủ linh vật: {realData.magicalDefense}\n";
        if (realData.spiritDefense > 0)
            description += $"+ Phòng thủ linh lực: {realData.spiritDefense}\n";
        if (realData.movementSpeed > 0)
            description += $"+ Tốc độ di chuyển: {realData.movementSpeed}\n";
        if (realData.spiritRange > 0)
            description += $"+ Phạp vi Linh lực: {realData.spiritRange}\n";
        if (realData.potential > 0)
            description += $"+ Tiềm năng: {realData.potential}\n";
        if (realData.skillPoints > 0)
            description += $"+ Điểm kỹ năng: {realData.skillPoints}\n";
        if (realData.combatPower > 0)
            description += $"+ Sức mạnh chiến đấu: {realData.combatPower}\n";
        if (realData.critDamage > 0)
            description += $"+ Sát thương chí mạng: {realData.critDamage}\n";
        if (realData.critRate > 0)
            description += $"+ Tỷ lệ chí mạng: {realData.critRate}\n";
        if (realData.evasion > 0)
            description += $"+ Né tránh: {realData.evasion}\n";
        if (realData.attackSpeed > 0)
            description += $"+ Tốc độ tấn công: {realData.attackSpeed}\n";
        if (realData.castSpeed > 0)
            description += $"+ Tốc độ thi triển: {realData.castSpeed}\n";
        if (realData.armorPenetration > 0)
            description += $"+ Xuyên phá giáp: {realData.armorPenetration}\n";
        // Remove trailing newline if exists
        if (description.EndsWith("\n"))
            description = description.Substring(0, description.Length - 1);

        return description;
    }


}
