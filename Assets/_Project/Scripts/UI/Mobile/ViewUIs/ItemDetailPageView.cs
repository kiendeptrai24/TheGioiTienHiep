using System;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
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
    private StatsData statsData;

    protected override void Awake()
    {
        statsData = GetComponentInParent<StatsData>();
    }

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
        essenceTypeTxt.text =
        inventoryItem.data is SkillData ? EnumTranslator.ToVietnamese(((SkillData)inventoryItem.data).raceType) :
        inventoryItem.data is TechniqueData ? EnumTranslator.ToVietnamese(((TechniqueData)inventoryItem.data).raceType) :
        inventoryItem.data is EquipmentData ? EnumTranslator.ToVietnamese(((EquipmentData)inventoryItem.data).raceType) :
        inventoryItem.data is HeroData ? EnumTranslator.ToVietnamese(((HeroData)inventoryItem.data).essenceType) :
        inventoryItem.data is PillData ? EnumTranslator.ToVietnamese(((PillData)inventoryItem.data).pillType) : "";
        itemIconImge.sprite = inventoryItem.data.itemIcon;

        if (inventoryItem.data is SkillData skillData) SetItemSkillData(skillData);
        else if (inventoryItem.data is TechniqueData techniqueData) SetItemTechniqueData(techniqueData);
        else if (inventoryItem.data is EquipmentData equipmentData) SetItemEquipmentData(equipmentData);
        else if (inventoryItem.data is HeroData heroData) SetItemHeroData(heroData);
        else if (inventoryItem.data is PillData pillData) SetItemPillData(pillData);
        else if (inventoryItem.data is ItemData itemData) SetItemData(itemData);
    }


    // Helper methods
    private void DisplayStat(string label, float value, bool isPercent = true)
    {
        value = isPercent ? value * 100 : value;
        if (value != 0) CreateItemDescriptionDetail(SetColor(label, value.ToString(), isPercent));
    }

    private void DisplayText(string label, string value, bool isPercent = false)
    {
        CreateItemDescriptionDetail(SetColor(label, value, isPercent)); 
    }
    private void DisplayBaseStat(ItemData data, bool isPercent = false)
    {
        DisplayStat("Increase Max Health", data.health, isPercent);
        DisplayStat("Increase Max Mana", data.mana, isPercent);
        DisplayStat("Increase Max Spirit", data.spirit, isPercent);
    }
    private void DisplayDamageStats(ItemData data, bool isPercent = false)
    {
        DisplayStat("Increase Physical Damage", data.physicalDamage, isPercent);
        DisplayStat("Increase Magical Damage", data.magicalDamage, isPercent);
        DisplayStat("Increase Spirit Damage", data.spiritDamage, isPercent);
    }

    private void DisplayDefenseStats(ItemData data, bool isPercent = false)
    {
        DisplayStat("Increase Physical Defense", data.physicalDefense, isPercent);
        DisplayStat("Increase Magical Defense", data.magicalDefense, isPercent);
        DisplayStat("Increase Spirit Defense", data.spiritDefense, isPercent);
    }

    public void SetItemData(ItemData itemData)
    {
        DisplayText("Quality Type", EnumTranslator.ToVietnamese(itemData.qualityType));
        DisplayDamageStats(itemData);
        DisplayDefenseStats(itemData);
    }

    public void SetItemEquipmentData(EquipmentData data)
    {
        DisplayText("Quality Type", EnumTranslator.ToVietnamese(data.qualityType));
        DisplayStat("Enhance Level", data.level, false);
        DisplayText("Request Realm", EnumTranslator.ToVietnamese(data.realmType));
        DisplayText("Element Type", EnumTranslator.ToVietnamese(data.elementType));
        DisplayText("Increase Point", data.potentialPoints.ToString());
        // Damage
        DisplayBaseStat(data, true);
        DisplayDamageStats(data, true);
        DisplayDefenseStats(data, true);
    }

    public void SetItemTechniqueData(TechniqueData data)
    {
        DisplayText("Quality Type", EnumTranslator.ToVietnamese(data.qualityType));
        DisplayStat("Enhance Level", data.enhanceLevel, false);
        DisplayText("Request Realm", EnumTranslator.ToVietnamese(data.realmType));
        DisplayText("Element Type", EnumTranslator.ToVietnamese(data.elementType));
        DisplayText("Increase Point", data.potentialPoints.ToString());
        DisplayBaseStat(data, true);
        DisplayDamageStats(data, true);
        DisplayDefenseStats(data, true);
    }

    public void SetItemHeroData(HeroData data)
    {
        DisplayText("Quality Type", EnumTranslator.ToVietnamese(data.qualityType));
        DisplayText("Realm", EnumTranslator.ToVietnamese(data.realmType));
        DisplayText("Element Type", EnumTranslator.ToVietnamese(data.elementType));
        DisplayText("Race Type", EnumTranslator.ToVietnamese(data.raceType));
        DisplayText("Essence Type", EnumTranslator.ToVietnamese(data.essenceType));
        DisplayText("Increase Point", data.potentialPoints.ToString());

        if (statsData == null) statsData = new StatsData();
        statsData.SetUpItem(data);

        DisplayText("Increase Max Health", statsData.Health.ToString());
        DisplayText("Increase Max Mana", statsData.Mana.ToString());
        DisplayText("Increase Max Spirit", statsData.Spirit.ToString());
        DisplayText("Increase Physical Damage", statsData.PhysicalDamage.ToString());
        DisplayText("Increase Magical Damage", statsData.MagicalDamage.ToString());
        DisplayText("Increase Spirit Damage", statsData.SpiritDamage.ToString());
        DisplayText("Increase Physical Defense", statsData.PhysicalDefense.ToString());
        DisplayText("Increase Magical Defense", statsData.MagicalDefense.ToString());
        DisplayText("Increase Spirit Defense", statsData.SpiritDefense.ToString());
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
        DisplayText("Request Realm", EnumTranslator.ToVietnamese(data.realmType));
        DisplayText("Increase Point", data.potentialPoints.ToString());
        DisplayBaseStat(data, true);
        DisplayDamageStats(data, true);
        DisplayDefenseStats(data, true);
    }

    private void SetItemPillData(PillData data)
    {
        CreateItemDescriptionDetail(SetColor("Pill Name", data.itemName, false));
        DisplayText("Pill Type", EnumTranslator.ToVietnamese(data.pillType));
        DisplayText("Quality Type", EnumTranslator.ToVietnamese(data.qualityType));
        DisplayText("Element Type", EnumTranslator.ToVietnamese(data.elementType));
        DisplayText("Request Realm", EnumTranslator.ToVietnamese(data.realmType));
        DisplayBaseStat(data);
        DisplayStat("Breakthrough Rate", data.rate);
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
        {"Request Realm", "Cảnh giới yêu cầu"},
        {"Attack Range", "Tầm đánh"},
        {"Health Cost", "Tiêu hao sinh lực"},
        { "Spirit Cost", "Tiêu hao linh thức"},
        { "Required Character Level", "Cấp nhân vật yêu cầu"},
        { "Learn Condition", "Điều kiện học"},
        { "Other Note", "Ghi chú khác"},
        { "Power Cost", "Tiêu hao Power"},
        { "Linh Thao Cost", "Tiêu hao Linh Thảo"},
        { "Mineral Cost", "Tiêu hao Khoáng Thạch"},
        { "Demon Core Cost", "Tiêu hao Yêu Đan"},
        { "Devil Core Cost", "Tiêu hao Ma Hạch"},
        { "Spirit Stone Cost", "Tiêu hao Linh Thạch"},
        { "Item Cost", "Tiêu hao vật phẩm khác"},
        { "Level", "Cấp"},
        { "Increase Point", "Tiềm năng điểm"},
        { "Increase Physical Damage Point", "Tăng sát thương Điểm Linh Thể"},
        { "Increase Magical Damage Point", "Tăng sát thương Điểm Linh Lực"},
        { "Increase Spirit Damage Point", "Tăng sát thương Điểm Linh Thức"},
        { "Increase Physical Defense Point", "Tăng phòng thủ Điểm Linh Thể"},
        { "Increase Magical Defense Point", "Tăng phòng thủ Điểm Linh Lực"},
        { "Increase Spirit Defense Point", "Tăng phòng thủ Điểm Linh Thức"},
        { "Essence Type", "Chủ tu"},
        { "Element Type", "Hệ"},
        { "Cooldown", "Thời gian hồi chiêu"},
        { "Mana Cost", "Tiêu hao linh lực"},
        { "Increase Max Health", "Tăng sinh lực tối đa"},
        { "Increase Max Mana", "Tăng linh lực tối đa"},
        { "Increase Max Spirit", "Tăng linh thức tối đa"},
        { "Increase Health Regen", "Tăng hồi sinh lực"},
        { "Increase Mana Regen", "Tăng hồi linh lực"},
        { "Increase Spirit Regen", "Tăng hồi linh thức"},
        { "Increase Ally Health Regen", "Tăng hồi sinh lực đồng minh"},
        { "Increase Ally Mana Regen", "Tăng hồi linh lực đồng minh"},
        { "Increase Ally Spirit Regen", "Tăng hồi linh thức đồng minh"},
        { "Increase Critical Damage", "Tăng sát thương chí mạng"},
        { "Increase Critical Rate", "Tăng tỉ lệ chí mạng"},
        { "Increase Reduce Critical Damage", "Tăng giảm sát thương chí mạng"},
        { "Increase Reduce Armor Penetration", "Tăng giảm xuyên giáp"},
        { "Increase Reduce True Damage", "Tăng giảm sát thương chuẩn"},
        { "Increase Reflect Damage", "Tăng phản đòn"},
        { "Increase Move Speed", "Tăng tốc độ di chuyển"},
        { "Increase Immune Ally Damage", "Tăng miễn sát thương đồng minh"},
        { "Increase Immune Ally Effects", "Tăng miễn hiệu ứng đồng minh"},
        { "Increase Immune All From Allies", "Tăng miễn tất cả từ đồng minh"},
        { "Increase Cleanse Ally Effects", "Tăng giải trừ hiệu ứng đồng minh"},
        { "Increase Grievous Wound", "Tăng vết thương sâu"},
        { "Increase Reduce Enemy Mana", "Tăng giảm linh lực đối phương"},
        { "Increase Reduce Enemy Spirit", "Tăng giảm linh thức đối phương"},
        { "Increase Weaken Target", "Tăng suy yếu"},
        { "Increase Paralyze Chance", "Tăng tỉ lệ tê liệt"},
        { "Increase Root Chance", "Tăng tỉ lệ vây khốn"},
        { "Increase Stun Chance", "Tăng tỉ lệ choáng"},
        { "Increase Silence Chance", "Tăng tỉ lệ câm lặng"},
        { "Increase Immune Damage", "Tăng miễn sát thương"},
        { "Increase Immune Effects", "Tăng miễn hiệu ứng"},
        { "Increase Immune All", "Tăng miễn tất cả"},
        { "Increase Reduce Effect Duration", "Tăng giảm thời hạn hiệu ứng"},
        { "Increase Effect Resistance", "Tăng kháng hiệu ứng"},
        { "Pill Name", "Tên thuốc"},
        { "Pill Type", "Loại thuốc"},
        { "Breakthrough Rate", "Tăng Tỷ lệ đột phá"},
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
