using System;
using TGTH.Mobile;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ItemSkillDetailPageView : IItemDetailPageView
{
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
    private LevelUpValidator levelUpValidator;
    protected override void Awake()
    {
        base.Awake();
        levelUpDatabase = LevelUpDatabase.Instance;
        levelUpValidator = LevelUpValidator.Instance;
        levelUpBtn.onClick.AddListener(OnLevelUpButtonClicked);
    }

    private void OnLevelUpButtonClicked()
    {
        if (!NetworkManager.Singleton.IsConnectedClient) return;
        if (levelUpDatabase == null) return;
        if (itemData == null) return;
        if(nextItemData == null)
        {
            TopNotificationUI.Instance.ShowNotification("Đã đạt cấp độ tối đa");
            return;
        }
        if (itemData is SkillData skillData)
        {
            ulong PlayerNetId = NetworkManager.Singleton.LocalClientId;
            levelUpValidator.RequestSkillEnhance(skillData.instanceId, skillData.itemId, PlayerNetId);
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
        nextItemData = levelUpDatabase.GetNextSkillEnhance(itemData.instanceId, itemData.enhanceLevel);

        if (nextItemData != null)
        {
            nextItemIcon.gameObject.SetActive(true);
            nextItemIcon.sprite = nextItemData.itemIcon;
            nextSkillnameTxt.text = nextItemData.itemName;
            nextRealmTxt.text = nextItemData.realm + "";
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
            description += $"+ Physical Damage: {skill.physicalDamage}\n";
        if (skill.magicalDamage > 0)
            description += $"+ Magical Damage: {skill.magicalDamage}\n";
        if (skill.spiritDamage > 0)
            description += $"+ Spirit Damage: {skill.spiritDamage}\n";

        // Base Defense Stats (from ItemData)
        if (skill.physicalDefense > 0)
            description += $"+ Physical Defense: {skill.physicalDefense}\n";
        if (skill.magicalDefense > 0)
            description += $"+ Magical Defense: {skill.magicalDefense}\n";
        if (skill.spiritDefense > 0)
            description += $"+ Spirit Defense: {skill.spiritDefense}\n";

        // Resource Cost
        if (skill.healthCost > 0)
            description += $"+ Health Cost: {skill.healthCost}\n";
        if (skill.manaCost > 0)
            description += $"+ Mana Cost: {skill.manaCost}\n";
        if (skill.spiritCost > 0)
            description += $"+ Spirit Cost: {skill.spiritCost}\n";

        // Combat Behavior
        if (skill.attackRange > 0)
            description += $"+ Attack Range: {skill.attackRange}\n";
        if (skill.cooldown > 0)
            description += $"+ Cooldown: {skill.cooldown}\n";
        if (skill.attackSpeed > 0)
            description += $"+ Attack Speed: {skill.attackSpeed}\n";

        // Offensive Stats Bonus
        if (skill.critDamage > 0)
            description += $"+ Crit Damage: {skill.critDamage}\n";
        if (skill.critRate > 0)
            description += $"+ Crit Rate: {skill.critRate}\n";
        if (skill.armorPenetration > 0)
            description += $"+ Armor Penetration: {skill.armorPenetration}\n";
        if (skill.trueDamage > 0)
            description += $"+ True Damage: {skill.trueDamage}\n";
        if (skill.lifeSteal > 0)
            description += $"+ Life Steal: {skill.lifeSteal}\n";

        // Defensive Stats Bonus
        if (skill.penetrationReduction > 0)
            description += $"+ Penetration Reduction: {skill.penetrationReduction}\n";
        if (skill.critDamageReduction > 0)
            description += $"+ Crit Damage Reduction: {skill.critDamageReduction}\n";
        if (skill.trueDamageReduction > 0)
            description += $"+ True Damage Reduction: {skill.trueDamageReduction}\n";

        // Resource Bonus
        if (skill.bonusHealth > 0)
            description += $"+ Bonus Health: {skill.bonusHealth}\n";
        if (skill.bonusMana > 0)
            description += $"+ Bonus Mana: {skill.bonusMana}\n";
        if (skill.bonusSpirit > 0)
            description += $"+ Bonus Spirit: {skill.bonusSpirit}\n"; ;

        // Remove trailing newline if exists
        if (description.EndsWith("\n"))
            description = description.Substring(0, description.Length - 1);

        return description;
    }
}
