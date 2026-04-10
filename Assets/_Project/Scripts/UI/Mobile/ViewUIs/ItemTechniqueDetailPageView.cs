using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ItemTechniqueDetailPageView : IItemDetailPageView
{
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
    protected override void Awake()
    {
        base.Awake();
        levelUpValidator = LevelUpValidator.Instance;
        levelUpDatabase = LevelUpDatabase.Instance;
        levelUpBtn.onClick.AddListener(OnLevelUpButtonClicked);
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
            description += $"+ Physical Damage: {technique.physicalDamage}\n";
        if (technique.magicalDamage > 0)
            description += $"+ Magical Damage: {technique.magicalDamage}\n";
        if (technique.spiritDamage > 0)
            description += $"+ Spirit Damage: {technique.spiritDamage}\n";

        // Base Defense Stats (from ItemData)
        if (technique.physicalDefense > 0)
            description += $"+ Physical Defense: {technique.physicalDefense}\n";
        if (technique.magicalDefense > 0)
            description += $"+ Magical Defense: {technique.magicalDefense}\n";
        if (technique.spiritDefense > 0)
            description += $"+ Spirit Defense: {technique.spiritDefense}\n";

        // Resource Cost
        if (technique.healthCost > 0)
            description += $"+ Health Cost: {technique.healthCost}\n";
        if (technique.manaCost > 0)
            description += $"+ Mana Cost: {technique.manaCost}\n";
        if (technique.spiritCost > 0)
            description += $"+ Spirit Cost: {technique.spiritCost}\n";

        // Combat Behavior
        if (technique.attackRange > 0)
            description += $"+ Attack Range: {technique.attackRange}\n";
        if (technique.cooldown > 0)
            description += $"+ Cooldown: {technique.cooldown}\n";
        if (technique.attackSpeed > 0)
            description += $"+ Attack Speed: {technique.attackSpeed}\n";

        // Offensive Stats Bonus
        if (technique.critDamage > 0)
            description += $"+ Crit Damage: {technique.critDamage}\n";
        if (technique.critRate > 0)
            description += $"+ Crit Rate: {technique.critRate}\n";
        if (technique.armorPenetration > 0)
            description += $"+ Armor Penetration: {technique.armorPenetration}\n";
        if (technique.trueDamage > 0)
            description += $"+ True Damage: {technique.trueDamage}\n";
        if (technique.lifeSteal > 0)
            description += $"+ Life Steal: {technique.lifeSteal}\n";

        // Defensive Stats Bonus
        if (technique.penetrationReduction > 0)
            description += $"+ Penetration Reduction: {technique.penetrationReduction}\n";
        if (technique.critDamageReduction > 0)
            description += $"+ Crit Damage Reduction: {technique.critDamageReduction}\n";
        if (technique.trueDamageReduction > 0)
            description += $"+ True Damage Reduction: {technique.trueDamageReduction}\n";

        // Resource Bonus
        if (technique.bonusHealth > 0)
            description += $"+ Bonus Health: {technique.bonusHealth}\n";
        if (technique.bonusMana > 0)
            description += $"+ Bonus Mana: {technique.bonusMana}\n";
        if (technique.bonusSpirit > 0)
            description += $"+ Bonus Spirit: {technique.bonusSpirit}\n"; ;

        // Remove trailing newline if exists
        if (description.EndsWith("\n"))
            description = description.Substring(0, description.Length - 1);

        return description;
    }


}
