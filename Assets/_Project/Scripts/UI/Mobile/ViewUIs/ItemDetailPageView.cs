using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDetailPageView : IItemDetailPageView
{
    [Header("Content")]
    [SerializeField] private TextMeshProUGUI itemNameTxt;
    [SerializeField] private TextMeshProUGUI realmTxt;
    [SerializeField] private TextMeshProUGUI qualityTypeTxt;
    [SerializeField] private TextMeshProUGUI essenceTypeTxt;
    [SerializeField] private Image itemIconImge;
    [SerializeField] private ItemDescriptionDetail itemDescriptionDetailPrefab;
    [SerializeField] private Transform content;

    protected override void Awake() => base.Awake();

    public override void HandleItemClicked(InventoryItem inventoryItem)
    {
        ResetItemDescription();
        SetData(inventoryItem);
    }

    private void ResetItemDescription()
    {
        foreach (var item in content.GetComponentsInChildren<ItemDescriptionDetail>())
            Destroy(item.gameObject);
    }

    private void SetData(InventoryItem inventoryItem)
    {
        if (inventoryItem == null || inventoryItem.data == null)
        {
            itemNameTxt.text = "";
            qualityTypeTxt.text = "";
            realmTxt.text = "";
            essenceTypeTxt.text = "";
            itemIconImge.sprite = null;
            return;
        }
        itemNameTxt.text = inventoryItem.data.itemName;
        qualityTypeTxt.text = EnumTranslator.ToVietnamese(inventoryItem.data.qualityType);
        realmTxt.text = EnumTranslator.ToVietnamese(inventoryItem.data.realmType);
        essenceTypeTxt.text = inventoryItem.data is SkillData ? EnumTranslator.ToVietnamese(((SkillData)inventoryItem.data).raceType) :
        inventoryItem.data is TechniqueData ? EnumTranslator.ToVietnamese(((TechniqueData)inventoryItem.data).raceType) :
        inventoryItem.data is EquitmentData ? EnumTranslator.ToVietnamese(((EquitmentData)inventoryItem.data).raceType) :
        inventoryItem.data is HeroData ? EnumTranslator.ToVietnamese(((HeroData)inventoryItem.data).essenceType) : "";
        itemIconImge.sprite = inventoryItem.data.itemIcon;

        if (inventoryItem.data is SkillData skillData) SetItemSkillData(skillData);
        else if (inventoryItem.data is TechniqueData techniqueData) SetItemTechniqueData(techniqueData);
        else if (inventoryItem.data is EquitmentData equipmentData) SetItemEquipmentData(equipmentData);
        else if (inventoryItem.data is HeroData heroData) SetItemHeroData(heroData);
        else if (inventoryItem.data is ItemData itemData) SetItemData(itemData);
    }

    // Helper methods
    private void DisplayStat(string label, float value, bool isPercent = true)
    {
        if (value != 0) CreateItemDescriptionDetail(SetColor(label, value.ToString(), isPercent));
    }

    private void DisplayText(string label, string value, bool isPercent = false)
    {
        CreateItemDescriptionDetail(SetColor(label, value, isPercent));
    }

    private void DisplayTextIfNotEmpty(string label, string value)
    {
        if (!string.IsNullOrEmpty(value)) CreateItemDescriptionDetail(SetColor(label, value, false));
    }

    private void DisplayDamageStats(ItemData data)
    {
        DisplayStat("Increase Physical Damage", data.physicalDamage);
        DisplayStat("Increase Magical Damage", data.magicalDamage);
        DisplayStat("Increase Spirit Damage", data.spiritDamage);
    }

    private void DisplayDefenseStats(ItemData data)
    {
        DisplayStat("Increase Physical Defense", data.physicalDefense);
        DisplayStat("Increase Magical Defense", data.magicalDefense);
        DisplayStat("Increase Spirit Defense", data.spiritDefense);
    }

    public void SetItemData(ItemData itemData)
    {
        DisplayText("Quality Type", EnumTranslator.ToVietnamese(itemData.qualityType));
        DisplayDamageStats(itemData);
        DisplayDefenseStats(itemData);
    }

    public void SetItemEquipmentData(EquitmentData data)
    {
        DisplayText("Quality Type", EnumTranslator.ToVietnamese(data.qualityType));
        DisplayStat("Enhance Level", data.level, false);
        DisplayText("Realm", EnumTranslator.ToVietnamese(data.realmType));
        DisplayText("Element Type", EnumTranslator.ToVietnamese(data.elementType));

        // Damage
        DisplayDamageStats(data);
        DisplayDefenseStats(data);
        DisplayStat("Increase Critical Damage", data.critDamage);
        DisplayStat("Increase Critical Rate", data.critRate);
        DisplayStat("Increase Life Steal", data.lifeSteal);
        DisplayStat("Increase Attack Speed", data.attackSpeed);

        // Health/Mana
        DisplayStat("Increase Max Health", data.maxHealth);
        DisplayStat("Increase Max Mana", data.maxMana);
        DisplayStat("Increase Max Spirit", data.maxSpirit);

        // Regen
        DisplayStat("Increase Health Regen", data.healthRegen);
        DisplayStat("Increase Mana Regen", data.manaRegen);
        DisplayStat("Increase Spirit Regen", data.spiritRegen);
        DisplayStat("Increase Ally Health Regen", data.allyHealthRegen);
        DisplayStat("Increase Ally Mana Regen", data.allyManaRegen);
        DisplayStat("Increase Ally Spirit Regen", data.allySpiritRegen);

        // Reduction
        DisplayStat("Increase Reduce Critical Damage", data.critDamageReduction);
        DisplayStat("Increase Reduce Armor Penetration", data.armorPenetrationReduction);
        DisplayStat("Increase Reduce True Damage", data.trueDamageReduction);

        // Other
        DisplayStat("Increase Reflect Damage", data.reflectDamage);
        DisplayStat("Increase Move Speed", data.moveSpeed);
        DisplayStat("Increase Immune Ally Damage", data.immuneAllyDamage);
        DisplayStat("Increase Immune Ally Effects", data.immuneAllyEffects);
        DisplayStat("Increase Immune All From Allies", data.immuneAllFromAllies);
        DisplayStat("Increase Cleanse Ally Effects", data.cleanseAllyEffects);
        DisplayStat("Increase Grievous Wound", data.grievousWound);
        DisplayStat("Increase Reduce Enemy Mana", data.reduceEnemyMana);
        DisplayStat("Increase Reduce Enemy Spirit", data.reduceEnemySpirit);
        DisplayStat("Increase Weaken Target", data.weakenTarget);
        DisplayStat("Increase Paralyze Chance", data.paralyzeChance);
        DisplayStat("Increase Root Chance", data.rootChance);
        DisplayStat("Increase Stun Chance", data.stunChance);
        DisplayStat("Increase Silence Chance", data.silenceChance);
        DisplayStat("Increase Immune Damage", data.immuneDamage);
        DisplayStat("Increase Immune Effects", data.immuneEffects);
        DisplayStat("Increase Immune All", data.immuneAll);
        DisplayStat("Increase Reduce Effect Duration", data.reduceEffectDuration);
        DisplayStat("Increase Effect Resistance", data.effectResistance);
    }

    public void SetItemTechniqueData(TechniqueData data)
    {
        DisplayText("Quality Type", EnumTranslator.ToVietnamese(data.qualityType));
        DisplayStat("Enhance Level", data.enhanceLevel, false);
        DisplayText("Realm", EnumTranslator.ToVietnamese(data.realm));
        DisplayText("Element Type", EnumTranslator.ToVietnamese(data.elementType));

        DisplayStat("Attack Range", data.attackRange, false);
        DisplayStat("Cooldown", data.cooldown, false);

        DisplayStat("Health Cost", data.healthCost);
        DisplayStat("Mana Cost", data.manaCost);
        DisplayStat("Spirit Cost", data.spiritCost);

        DisplayStat("Required Character Level", data.requiredCharacterLevel, false);
        DisplayTextIfNotEmpty("Learn Condition", data.learnCondition);

        DisplayStat("Power Cost", data.powerCost);
        DisplayStat("Linh Thao Cost", data.linhThaoCost);
        DisplayStat("Mineral Cost", data.khoangThachCost);
        DisplayStat("Demon Core Cost", data.yeuDanCost);
        DisplayStat("Devil Core Cost", data.maHachCost);
        DisplayStat("Spirit Stone Cost", data.linhThachCost);
        DisplayStat("Item Cost", data.itemCost);

        // Damage
        DisplayDamageStats(data);
        DisplayDefenseStats(data);
        DisplayStat("Increase Crit Damage", data.critDamage);
        DisplayStat("Increase Crit Rate", data.critRate);
        DisplayStat("Increase Armor Penetration", data.armorPenetration);
        DisplayStat("Increase True Damage", data.trueDamage);
        DisplayStat("Increase Life Steal", data.lifeSteal);
        DisplayStat("Increase Attack Speed", data.attackSpeed);

        // Defense
        DisplayStat("Reduce Penetration Damage", data.penetrationReduction);
        DisplayStat("Reduce Crit Damage", data.critDamageReduction);
        DisplayStat("Reduce True Damage", data.trueDamageReduction);

        // Resource
        DisplayStat("Bonus Health", data.bonusHealth);
        DisplayStat("Bonus Mana", data.bonusMana);
        DisplayStat("Bonus Spirit", data.bonusSpirit);

        // Summary
        DisplayStat("Total Quality And Level", data.totalQualityAndLevel);
        DisplayStat("Stat Count", data.statCount);
    }

    public void SetItemHeroData(HeroData data)
    {
        DisplayText("Quality Type", EnumTranslator.ToVietnamese(data.qualityType));
        DisplayText("Realm", EnumTranslator.ToVietnamese(data.realmType));
        DisplayText("Element Type", EnumTranslator.ToVietnamese(data.elementType));
        DisplayText("Race Type", EnumTranslator.ToVietnamese(data.raceType));
        DisplayText("Essence Type", EnumTranslator.ToVietnamese(data.essenceType));

        DisplayStat("Attack Range", data.attackRange, false);
        DisplayStat("Bonus Health", data.health);
        DisplayStat("Bonus Mana", data.mana);
        DisplayStat("Bonus Spirit", data.spirit);

        DisplayStat("Increase Physical Damage Point", data.physicalDamagePoint, false);
        DisplayStat("Increase Magical Damage Point", data.magicalDamagePoint, false);
        DisplayStat("Increase Spirit Damage Point", data.spiritDamagePoint, false);
        DisplayStat("Increase Physical Defense Point", data.physicalDefensePoint, false);
        DisplayStat("Increase Magical Defense Point", data.magicalDefensePoint, false);
        DisplayStat("Increase Spirit Defense Point", data.spiritDefensePoint, false);

        DisplayDamageStats(data);
        DisplayDefenseStats(data);
    }

    public void SetItemSkillData(SkillData data)
    {
        // Meta
        CreateItemDescriptionDetail(SetColor("Skill Name", data.itemName, false));
        DisplayText("Skill Type", EnumTranslator.ToVietnamese(data.skillType));
        DisplayText("Quality Type", EnumTranslator.ToVietnamese(data.qualityType));
        DisplayStat("Enhance Level", data.enhanceLevel, false);
        DisplayText("Race Type", EnumTranslator.ToVietnamese(data.raceType));
        DisplayText("Main Essence", EnumTranslator.ToVietnamese(data.mainEssence));
        DisplayText("Element Type", EnumTranslator.ToVietnamese(data.elementType));
        DisplayText("Realm", EnumTranslator.ToVietnamese(data.realm));

        // Combat
        DisplayStat("Attack Range", data.attackRange, false);
        DisplayStat("Cooldown", data.cooldown, false);

        // Resource Cost
        DisplayStat("Health Cost", data.healthCost);
        DisplayStat("Mana Cost", data.manaCost);
        DisplayStat("Spirit Cost", data.spiritCost);

        // Learn Conditions
        DisplayStat("Required Character Level", data.requiredCharacterLevel, false);
        DisplayTextIfNotEmpty("Learn Condition", data.learnCondition);
        DisplayTextIfNotEmpty("Other Note", data.otherNote);

        // Upgrade Materials
        DisplayStat("Power Cost", data.powerCost);
        DisplayStat("Linh Thao Cost", data.linhThaoCost);
        DisplayStat("Mineral Cost", data.khoangThachCost);
        DisplayStat("Demon Core Cost", data.yeuDanCost);
        DisplayStat("Devil Core Cost", data.maHachCost);
        DisplayStat("Spirit Stone Cost", data.linhThachCost);
        DisplayStat("Item Cost", data.itemCost);

        // Damage
        DisplayDamageStats(data);
        DisplayDefenseStats(data);
        DisplayStat("Increase Crit Damage", data.critDamage);
        DisplayStat("Increase Crit Rate", data.critRate);
        DisplayStat("Increase Armor Penetration", data.armorPenetration);
        DisplayStat("Increase True Damage", data.trueDamage);
        DisplayStat("Increase Life Steal", data.lifeSteal);
        DisplayStat("Increase Attack Speed", data.attackSpeed);

        // Defense
        DisplayStat("Reduce Penetration Damage", data.penetrationReduction);
        DisplayStat("Reduce Crit Damage", data.critDamageReduction);
        DisplayStat("Reduce True Damage", data.trueDamageReduction);

        // Resource
        DisplayStat("Bonus Health", data.bonusHealth);
        DisplayStat("Bonus Mana", data.bonusMana);
        DisplayStat("Bonus Spirit", data.bonusSpirit);

        // Summary
        DisplayStat("Total Quality And Level", data.totalQualityAndLevel);
        DisplayStat("Stat Count", data.statCount);
    }

    private static readonly System.Collections.Generic.Dictionary<string, string> labelVi = new()
    {
        {"Increase Physical Damage", "Tăng sát thương Linh Thể"},
        {"Increase Magical Damage", "Tăng sát thương phép"},
        {"Increase Spirit Damage", "Tăng sát thương linh lực"},
        {"Increase Physical Defense", "Tăng phòng thủ Linh Thể"},
        {"Increase Magical Defense", "Tăng phòng thủ phép"},
        {"Increase Spirit Defense", "Tăng phòng thủ linh lực"},
        {"Increase Crit Damage", "Tăng sát thương chí mạng"},
        {"Increase Crit Rate", "Tăng tỉ lệ chí mạng"},
        {"Increase Armor Penetration", "Tăng xuyên giáp"},
        {"Increase True Damage", "Tăng sát thương chuẩn"},
        {"Increase Life Steal", "Tăng hút máu"},
        {"Increase Attack Speed", "Tăng tốc độ đánh"},
        {"Reduce Penetration Damage", "Giảm sát thương xuyên giáp"},
        {"Reduce Crit Damage", "Giảm sát thương chí mạng"},
        {"Reduce True Damage", "Giảm sát thương chuẩn"},
        {"Bonus Health", "Tăng sinh lực"},
        {"Bonus Mana", "Tăng linh lực"},
        {"Bonus Spirit", "Tăng linh thức"},
        {"Total Quality And Level", "Tổng phẩm + cấp"},
        {"Stat Count", "Số chỉ số kích hoạt"},
        {"Skill Name", "Tên kỹ năng"},
        {"Skill Type", "Loại kỹ năng"},
        {"Quality Type", "Phẩm chất"},
        {"Enhance Level", "Cường hóa"},
        {"Race Type", "Chủng tộc"},
        {"Main Essence", "Chủ tu"},
        {"Realm", "Cảnh giới"},
        {"Attack Range", "Tầm đánh"},
        {"Health Cost", "Tiêu hao sinh lực"},
        {"Spirit Cost", "Tiêu hao linh thức"},
        {"Required Character Level", "Cấp nhân vật yêu cầu"},
        {"Learn Condition", "Điều kiện học"},
        {"Other Note", "Ghi chú khác"},
        {"Power Cost", "Tiêu hao Power"},
        {"Linh Thao Cost", "Tiêu hao Linh Thảo"},
        {"Mineral Cost", "Tiêu hao Khoáng Thạch"},
        {"Demon Core Cost", "Tiêu hao Yêu Đan"},
        {"Devil Core Cost", "Tiêu hao Ma Hạch"},
        {"Spirit Stone Cost", "Tiêu hao Linh Thạch"},
        {"Item Cost", "Tiêu hao vật phẩm khác"},
        {"Level", "Cấp"},
        {"Increase Physical Damage Point", "Tăng sát thương Điểm Linh Thể"},
        {"Increase Magical Damage Point", "Tăng sát thương Điểm Linh Lực"},
        {"Increase Spirit Damage Point", "Tăng sát thương Điểm Linh Thức"},
        {"Increase Physical Defense Point", "Tăng phòng thủ Điểm Linh Thể"},
        {"Increase Magical Defense Point", "Tăng phòng thủ Điểm Linh Lực"},
        {"Increase Spirit Defense Point", "Tăng phòng thủ Điểm Linh Thức"},
        {"Essence Type", "Chủ tu"},
        {"Element Type", "Hệ"},
        {"Cooldown", "Thời gian hồi chiêu"},
        {"Mana Cost", "Tiêu hao linh lực"},
        {"Increase Max Health", "Tăng sinh lực tối đa"},
        {"Increase Max Mana", "Tăng linh lực tối đa"},
        {"Increase Max Spirit", "Tăng linh thức tối đa"},
        {"Increase Health Regen", "Tăng hồi sinh lực"},
        {"Increase Mana Regen", "Tăng hồi linh lực"},
        {"Increase Spirit Regen", "Tăng hồi linh thức"},
        {"Increase Ally Health Regen", "Tăng hồi sinh lực đồng minh"},
        {"Increase Ally Mana Regen", "Tăng hồi linh lực đồng minh"},
        {"Increase Ally Spirit Regen", "Tăng hồi linh thức đồng minh"},
        {"Increase Critical Damage", "Tăng sát thương chí mạng"},
        {"Increase Critical Rate", "Tăng tỉ lệ chí mạng"},
        {"Increase Reduce Critical Damage", "Tăng giảm sát thương chí mạng"},
        {"Increase Reduce Armor Penetration", "Tăng giảm xuyên giáp"},
        {"Increase Reduce True Damage", "Tăng giảm sát thương chuẩn"},
        {"Increase Reflect Damage", "Tăng phản đòn"},
        {"Increase Move Speed", "Tăng tốc độ di chuyển"},
        {"Increase Immune Ally Damage", "Tăng miễn sát thương đồng minh"},
        {"Increase Immune Ally Effects", "Tăng miễn hiệu ứng đồng minh"},
        {"Increase Immune All From Allies", "Tăng miễn tất cả từ đồng minh"},
        {"Increase Cleanse Ally Effects", "Tăng giải trừ hiệu ứng đồng minh"},
        {"Increase Grievous Wound", "Tăng vết thương sâu"},
        {"Increase Reduce Enemy Mana", "Tăng giảm linh lực đối phương"},
        {"Increase Reduce Enemy Spirit", "Tăng giảm linh thức đối phương"},
        {"Increase Weaken Target", "Tăng suy yếu"},
        {"Increase Paralyze Chance", "Tăng tỉ lệ tê liệt"},
        {"Increase Root Chance", "Tăng tỉ lệ vây khốn"},
        {"Increase Stun Chance", "Tăng tỉ lệ choáng"},
        {"Increase Silence Chance", "Tăng tỉ lệ câm lặng"},
        {"Increase Immune Damage", "Tăng miễn sát thương"},
        {"Increase Immune Effects", "Tăng miễn hiệu ứng"},
        {"Increase Immune All", "Tăng miễn tất cả"},
        {"Increase Reduce Effect Duration", "Tăng giảm thời hạn hiệu ứng"},
        {"Increase Effect Resistance", "Tăng kháng hiệu ứng"},
    };

    private string SetColor(string label, string value, bool isPercent = true)
    {
        string viLabel = labelVi.ContainsKey(label) ? labelVi[label] : label;
        return isPercent ? $"{viLabel}: <color=#00FF00>{value}%</color>" : $"{viLabel}: <color=#00FF00>{value}</color>";
    }

    private void CreateItemDescriptionDetail(string description)
    {
        ItemDescriptionDetail itemDetail = Instantiate(itemDescriptionDetailPrefab, content);
        itemDetail.SetDescription(description);
    }
}
