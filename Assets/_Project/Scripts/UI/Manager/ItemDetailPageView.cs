using System;
using TGTH.Mobile;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDetailPageView : TGTHMonoBehaviour
{
    [Header("Content")]
    [SerializeField] private TextMeshProUGUI itemNameTxt;
    [SerializeField] private TextMeshProUGUI realmTxt;
    [SerializeField] private TextMeshProUGUI qualityTypeTxt;
    [SerializeField] private Image itemIconImge;
    [SerializeField] private ItemDescriptionDetail itemDescriptionDetailPrefab;
    [SerializeField] private Transform content;
    protected override void Awake()
    {
        base.Awake();
    }
    public void HandleItemClicked(InventoryItem inventoryItem)
    {
        ResetItemDescription();
        SetData(inventoryItem);
    }

    private void ResetItemDescription()
    {
        foreach (var item in content.GetComponentsInChildren<ItemDescriptionDetail>())
        {
            Destroy(item.gameObject);
        }
    }

    private void SetData(InventoryItem inventoryItem)
    {

        itemNameTxt.text = inventoryItem.data.itemName;
        qualityTypeTxt.text = inventoryItem.data.qualityType.ToString();
        //realmTxt.text = inventoryItem.data.cultivationStage.ToString();
        itemIconImge.sprite = inventoryItem.data.itemIcon;
        if (inventoryItem.data is EquitmentData itemEquitmentData)
        {
            SetItemEquipmentData(itemEquitmentData);
        }
        else if (inventoryItem.data is TechniqueData itemTechniqueData)
        {
            SetItemTechniqueData(itemTechniqueData);
        }
        else if (inventoryItem.data is SkillData itemSkillData)
        {
            SetItemSkillData(itemSkillData);
        }
        else if (inventoryItem.data is HeroData itemHeroData)
        {
            SetItemHeroData(itemHeroData);
        }
        else if (inventoryItem.data is ItemData itemData)
        {
            SetItemData(itemData);
        }
        
    }
    public void SetItemData(ItemData itemData)
    {
        CreateItemDescriptionDetail(SetColor("Increase Physical Damage", itemData.physicalDamage.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Magical Damage", itemData.magicalDamage.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Spirit Damage", itemData.spiritDamage.ToString()));

        CreateItemDescriptionDetail(SetColor("Increase Physical Defense", itemData.physicalDefense.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Magical Defense", itemData.magicalDefense.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Spirit Defense", itemData.spiritDefense.ToString()));
    }
    public void SetItemEquipmentData(EquitmentData itemEquipmentData)
    {
        CreateItemDescriptionDetail(SetColor("Increase Physical Damage", itemEquipmentData.physicalDamage.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Magical Damage", itemEquipmentData.magicalDamage.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Spirit Damage", itemEquipmentData.spiritDamage.ToString()));

        CreateItemDescriptionDetail(SetColor("Increase Physical Defense", itemEquipmentData.physicalDefense.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Magical Defense", itemEquipmentData.magicalDefense.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Spirit Defense", itemEquipmentData.spiritDefense.ToString()));

        CreateItemDescriptionDetail(SetColor("Increase Critical Damage", itemEquipmentData.critDamage.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Critical Rate", itemEquipmentData.critRate.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Life Steal", itemEquipmentData.lifeSteal.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Attack Speed", itemEquipmentData.attackSpeed.ToString()));

        CreateItemDescriptionDetail(SetColor("Increase Max Health", itemEquipmentData.maxHealth.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Max Mana", itemEquipmentData.maxMana.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Max Spirit", itemEquipmentData.maxSpirit.ToString()));

        CreateItemDescriptionDetail(SetColor("Increase Health Regen", itemEquipmentData.healthRegen.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Mana Regen", itemEquipmentData.manaRegen.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Spirit Regen", itemEquipmentData.spiritRegen.ToString()));

        CreateItemDescriptionDetail(SetColor("Increase Ally Health Regen", itemEquipmentData.allyHealthRegen.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Ally Mana Regen", itemEquipmentData.allyManaRegen.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Ally Spirit Regen", itemEquipmentData.allySpiritRegen.ToString()));

        CreateItemDescriptionDetail(SetColor("Increase Reduce Critical Damage", itemEquipmentData.critDamageReduction.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Reduce Armor Penetration", itemEquipmentData.armorPenetrationReduction.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Reduce True Damage", itemEquipmentData.trueDamageReduction.ToString()));

        CreateItemDescriptionDetail(SetColor("Increase Reflect Damage", itemEquipmentData.reflectDamage.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Move Speed", itemEquipmentData.moveSpeed.ToString()));

        CreateItemDescriptionDetail(SetColor("Increase Immune Ally Damage", itemEquipmentData.immuneAllyDamage.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Immune Ally Effects", itemEquipmentData.immuneAllyEffects.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Immune All From Allies", itemEquipmentData.immuneAllFromAllies.ToString()));

        CreateItemDescriptionDetail(SetColor("Increase Cleanse Ally Effects", itemEquipmentData.cleanseAllyEffects.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Grievous Wound", itemEquipmentData.grievousWound.ToString()));

        CreateItemDescriptionDetail(SetColor("Increase Reduce Enemy Mana", itemEquipmentData.reduceEnemyMana.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Reduce Enemy Spirit", itemEquipmentData.reduceEnemySpirit.ToString()));

        CreateItemDescriptionDetail(SetColor("Increase Weaken Target", itemEquipmentData.weakenTarget.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Paralyze Chance", itemEquipmentData.paralyzeChance.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Root Chance", itemEquipmentData.rootChance.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Stun Chance", itemEquipmentData.stunChance.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Silence Chance", itemEquipmentData.silenceChance.ToString()));

        CreateItemDescriptionDetail(SetColor("Increase Immune Damage", itemEquipmentData.immuneDamage.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Immune Effects", itemEquipmentData.immuneEffects.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Immune All", itemEquipmentData.immuneAll.ToString()));

        CreateItemDescriptionDetail(SetColor("Increase Reduce Effect Duration", itemEquipmentData.reduceEffectDuration.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Effect Resistance", itemEquipmentData.effectResistance.ToString()));
    }
    public void SetItemTechniqueData(TechniqueData itemTechniqueData)
    {
        // Meta
        // CreateItemDescriptionDetail(SetColor("Quality Type", itemTechniqueData.qualityType.ToString()));
        // CreateItemDescriptionDetail(SetColor("Enhance Level", itemTechniqueData.enhanceLevel.ToString()));
        // CreateItemDescriptionDetail(SetColor("Realm", itemTechniqueData.realm.ToString()));

        // Combat
        CreateItemDescriptionDetail(SetColor("Attack Range", itemTechniqueData.attackRange.ToString()));
        CreateItemDescriptionDetail(SetColor("Cooldown", itemTechniqueData.cooldown.ToString()));
        // CreateItemDescriptionDetail(SetColor("Special Effect", itemTechniqueData.specialEffect));

        // Resource Cost
        CreateItemDescriptionDetail(SetColor("Health Cost", itemTechniqueData.healthCost.ToString()));
        CreateItemDescriptionDetail(SetColor("Mana Cost", itemTechniqueData.manaCost.ToString()));
        CreateItemDescriptionDetail(SetColor("Spirit Cost", itemTechniqueData.spiritCost.ToString()));

        // Learn Conditions
        CreateItemDescriptionDetail(SetColor("Required Character Level", itemTechniqueData.requiredCharacterLevel.ToString()));
        CreateItemDescriptionDetail(SetColor("Learn Condition", itemTechniqueData.learnCondition));

        // Upgrade Materials
        CreateItemDescriptionDetail(SetColor("Power Cost", itemTechniqueData.powerCost.ToString()));
        CreateItemDescriptionDetail(SetColor("Linh Thao Cost", itemTechniqueData.lthaoCost.ToString()));
        CreateItemDescriptionDetail(SetColor("Mineral Cost", itemTechniqueData.mineralCost.ToString()));
        CreateItemDescriptionDetail(SetColor("Demon Core Cost", itemTechniqueData.demonCoreCost.ToString()));
        CreateItemDescriptionDetail(SetColor("Devil Core Cost", itemTechniqueData.devilCoreCost.ToString()));
        CreateItemDescriptionDetail(SetColor("Spirit Stone Cost", itemTechniqueData.spiritStoneCost.ToString()));
        CreateItemDescriptionDetail(SetColor("Item Cost", itemTechniqueData.itemCost.ToString()));

        // Offensive Stats Bonus
        CreateItemDescriptionDetail(SetColor("Increase Physical Damage", itemTechniqueData.physicalDamage.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Magical Damage", itemTechniqueData.magicalDamage.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Spirit Damage", itemTechniqueData.spiritDamage.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Physical Defense", itemTechniqueData.physicalDefense.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Magical Defense", itemTechniqueData.magicalDefense.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Spirit Defense", itemTechniqueData.spiritDefense.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Crit Damage", itemTechniqueData.critDamage.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Crit Rate", itemTechniqueData.critRate.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Armor Penetration", itemTechniqueData.armorPenetration.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase True Damage", itemTechniqueData.trueDamage.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Life Steal", itemTechniqueData.lifeSteal.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Attack Speed", itemTechniqueData.attackSpeed.ToString()));

        // Defensive Stats Bonus
        CreateItemDescriptionDetail(SetColor("Reduce Penetration Damage", itemTechniqueData.penetrationReduction.ToString()));
        CreateItemDescriptionDetail(SetColor("Reduce Crit Damage", itemTechniqueData.critDamageReduction.ToString()));
        CreateItemDescriptionDetail(SetColor("Reduce True Damage", itemTechniqueData.trueDamageReduction.ToString()));

        // Resource Bonus
        CreateItemDescriptionDetail(SetColor("Bonus Health", itemTechniqueData.bonusHealth.ToString()));
        CreateItemDescriptionDetail(SetColor("Bonus Mana", itemTechniqueData.bonusMana.ToString()));
        CreateItemDescriptionDetail(SetColor("Bonus Spirit", itemTechniqueData.bonusSpirit.ToString()));

        // Summary
        CreateItemDescriptionDetail(SetColor("Total Quality And Level", itemTechniqueData.totalQualityAndLevel.ToString()));
        CreateItemDescriptionDetail(SetColor("Stat Count", itemTechniqueData.statCount.ToString()));
    }
    public void SetItemHeroData(HeroData itemHeroData)
    {
        CreateItemDescriptionDetail(SetColor("Level", itemHeroData.level.ToString()));
        CreateItemDescriptionDetail(SetColor("Attack Range", itemHeroData.attackRange.ToString()));
        CreateItemDescriptionDetail(SetColor("Health", itemHeroData.health.ToString()));
        CreateItemDescriptionDetail(SetColor("Mana", itemHeroData.mana.ToString()));
        CreateItemDescriptionDetail(SetColor("Spirit", itemHeroData.spirit.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Physical Damage", itemHeroData.physicalDamage.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Magical Damage", itemHeroData.magicalDamage.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Spirit Damage", itemHeroData.spiritDamage.ToString()));
        CreateItemDescriptionDetail(SetColor("Physical Damage Point", itemHeroData.physicalDamagePoint.ToString()));
        CreateItemDescriptionDetail(SetColor("Magical Damage Point", itemHeroData.magicalDamagePoint.ToString()));
        CreateItemDescriptionDetail(SetColor("Spirit Damage Point", itemHeroData.spiritDamagePoint.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Physical Defense", itemHeroData.physicalDefense.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Magical Defense", itemHeroData.magicalDefense.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Spirit Defense", itemHeroData.spiritDefense.ToString()));
        CreateItemDescriptionDetail(SetColor("Physical Defense Point", itemHeroData.physicalDefensePoint.ToString()));
        CreateItemDescriptionDetail(SetColor("Magical Defense Point", itemHeroData.magicalDefensePoint.ToString()));
        CreateItemDescriptionDetail(SetColor("Spirit Defense Point", itemHeroData.spiritDefensePoint.ToString()));
        CreateItemDescriptionDetail(SetColor("Race Type", itemHeroData.raceType.ToString()));
        CreateItemDescriptionDetail(SetColor("Essence Type", itemHeroData.essenceType.ToString()));
        CreateItemDescriptionDetail(SetColor("Element Type", itemHeroData.elementType.ToString()));
        // Nếu muốn hiển thị thêm danh sách kỹ năng hoặc công pháp, có thể lặp qua skillDatas/techniqueDatas ở đây
    }

    public void SetItemSkillData(SkillData itemSkillData)
    {
        // Meta
        // CreateItemDescriptionDetail(SetColor("Skill Name", itemSkillData.itemName));
        // CreateItemDescriptionDetail(SetColor("Skill Type", itemSkillData.skillType.ToString()));
        // CreateItemDescriptionDetail(SetColor("Quality Type", itemSkillData.qualityType.ToString()));
        CreateItemDescriptionDetail(SetColor("Enhance Level", itemSkillData.enhanceLevel.ToString()));
        // CreateItemDescriptionDetail(SetColor("Race Type", itemSkillData.raceType.ToString()));
        // CreateItemDescriptionDetail(SetColor("Main Essence", itemSkillData.mainEssence.ToString()));
        // CreateItemDescriptionDetail(SetColor("Element Type", itemSkillData.elementType.ToString()));
        // CreateItemDescriptionDetail(SetColor("Realm", itemSkillData.realm.ToString()));

        // Combat
        CreateItemDescriptionDetail(SetColor("Attack Range", itemSkillData.attackRange.ToString()));
        CreateItemDescriptionDetail(SetColor("Cooldown", itemSkillData.cooldown.ToString()));
        // CreateItemDescriptionDetail(SetColor("Special Effect", itemSkillData.specialEffect));

        // Resource Cost
        CreateItemDescriptionDetail(SetColor("Health Cost", itemSkillData.healthCost.ToString()));
        CreateItemDescriptionDetail(SetColor("Mana Cost", itemSkillData.manaCost.ToString()));
        CreateItemDescriptionDetail(SetColor("Spirit Cost", itemSkillData.spiritCost.ToString()));

        // Learn Conditions
        CreateItemDescriptionDetail(SetColor("Required Character Level", itemSkillData.requiredCharacterLevel.ToString()));
        CreateItemDescriptionDetail(SetColor("Learn Condition", itemSkillData.learnCondition));
        CreateItemDescriptionDetail(SetColor("Other Note", itemSkillData.otherNote));

        // Upgrade Materials
        CreateItemDescriptionDetail(SetColor("Power Cost", itemSkillData.powerCost.ToString()));
        CreateItemDescriptionDetail(SetColor("Linh Thao Cost", itemSkillData.lthaoCost.ToString()));
        CreateItemDescriptionDetail(SetColor("Mineral Cost", itemSkillData.mineralCost.ToString()));
        CreateItemDescriptionDetail(SetColor("Demon Core Cost", itemSkillData.demonCoreCost.ToString()));
        CreateItemDescriptionDetail(SetColor("Devil Core Cost", itemSkillData.devilCoreCost.ToString()));
        CreateItemDescriptionDetail(SetColor("Spirit Stone Cost", itemSkillData.spiritStoneCost.ToString()));
        CreateItemDescriptionDetail(SetColor("Item Cost", itemSkillData.itemCost.ToString()));

        // Damage Bonus
        CreateItemDescriptionDetail(SetColor("Increase Physical Damage", itemSkillData.physicalDamage.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Magical Damage", itemSkillData.magicalDamage.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Spirit Damage", itemSkillData.spiritDamage.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Physical Defense", itemSkillData.physicalDefense.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Magical Defense", itemSkillData.magicalDefense.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Spirit Defense", itemSkillData.spiritDefense.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Crit Damage", itemSkillData.critDamage.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Crit Rate", itemSkillData.critRate.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Armor Penetration", itemSkillData.armorPenetration.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase True Damage", itemSkillData.trueDamage.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Life Steal", itemSkillData.lifeSteal.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Attack Speed", itemSkillData.attackSpeed.ToString()));

        // Defense Bonus
        CreateItemDescriptionDetail(SetColor("Reduce Penetration Damage", itemSkillData.penetrationReduction.ToString()));
        CreateItemDescriptionDetail(SetColor("Reduce Crit Damage", itemSkillData.critDamageReduction.ToString()));
        CreateItemDescriptionDetail(SetColor("Reduce True Damage", itemSkillData.trueDamageReduction.ToString()));

        // Resource Bonus
        CreateItemDescriptionDetail(SetColor("Bonus Health", itemSkillData.bonusHealth.ToString()));
        CreateItemDescriptionDetail(SetColor("Bonus Mana", itemSkillData.bonusMana.ToString()));
        CreateItemDescriptionDetail(SetColor("Bonus Spirit", itemSkillData.bonusSpirit.ToString()));

        // Summary
        CreateItemDescriptionDetail(SetColor("Total Quality And Level", itemSkillData.totalQualityAndLevel.ToString()));
        CreateItemDescriptionDetail(SetColor("Stat Count", itemSkillData.statCount.ToString()));
    }
    // Bảng chuyển đổi tiếng Việt cho các label
    private static readonly System.Collections.Generic.Dictionary<string, string> labelVi = new System.Collections.Generic.Dictionary<string, string>()
    {
        {"Increase Physical Damage", "Tăng sát thương vật lý"},
        {"Increase Magical Damage", "Tăng sát thương phép"},
        {"Increase Spirit Damage", "Tăng sát thương linh lực"},
        {"Increase Physical Defense", "Tăng phòng thủ vật lý"},
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
        {"Skill Power", "Sức mạnh kỹ năng"},
        {"Mana Cost", "Tiêu hao linh lực"},
        {"Cooldown", "Thời gian hồi chiêu"},
        {"Technique Name", "Tên công pháp"},
        {"Technique Type", "Loại công pháp"},
        {"Quality Type", "Phẩm chất"},
        {"Enhance Level", "Cường hóa"},
        {"Race Type", "Chủng tộc"},
        {"Main Essence", "Chủ tu"},
        {"Element Type", "Ngũ hành"},
        {"Realm", "Cảnh giới"},
        {"Attack Range", "Tầm đánh"},
        {"Special Effect", "Hiệu ứng đặc biệt"},
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
    };

    private string SetColor(string label, string value)
    {
        string viLabel = labelVi.ContainsKey(label) ? labelVi[label] : label;
        string result = $"{viLabel}: <color=#00FF00>{value}%</color>";
        return result;
    }
    private void CreateItemDescriptionDetail(string description)
    {
        ItemDescriptionDetail itemdetail = Instantiate(itemDescriptionDetailPrefab, content);
        itemdetail.SetDescription(description);
    }
}
