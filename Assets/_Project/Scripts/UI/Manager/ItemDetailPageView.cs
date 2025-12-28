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
        realmTxt.text = inventoryItem.data.cultivationStage.ToString();
        itemIconImge.sprite = inventoryItem.data.itemIcon;
        if(inventoryItem.data is ItemEquitmentData itemEquitmentData)
        {
            SetItemEquipmentData(itemEquitmentData);
        }
        else if(inventoryItem.data is ItemData itemData)
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
    public void SetItemEquipmentData(ItemEquitmentData itemEquipmentData)
    {
        CreateItemDescriptionDetail(SetColor("Increase Physical Damage", itemEquipmentData.physicalDamage.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Magical Damage", itemEquipmentData.magicalDamage.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Spirit Damage", itemEquipmentData.spiritDamage.ToString()));

        CreateItemDescriptionDetail(SetColor("Increase Physical Defense", itemEquipmentData.physicalDefense.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Magical Defense", itemEquipmentData.magicalDefense.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Spirit Defense", itemEquipmentData.spiritDefense.ToString()));

        CreateItemDescriptionDetail(SetColor("Increase Critical Damage", itemEquipmentData.criticalDamage.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Critical Rate", itemEquipmentData.criticalRate.ToString()));
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

        CreateItemDescriptionDetail(SetColor("Increase Reduce Critical Damage", itemEquipmentData.reduceCritDamage.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Reduce Armor Penetration", itemEquipmentData.reduceArmorPen.ToString()));
        CreateItemDescriptionDetail(SetColor("Increase Reduce True Damage", itemEquipmentData.reduceTrueDamage.ToString()));

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
    private string SetColor(string label, string value)
    {
        // string result =$"<color=#00FF00>{value}:</color> {data}<color=#00FF00>%</color>";
        string result =$"{label}: <color=#00FF00>{value}%</color>";
        return result;
    }
    private void CreateItemDescriptionDetail(string description)
    {
        ItemDescriptionDetail itemdetail = Instantiate(itemDescriptionDetailPrefab,content);
        itemdetail.SetDescription(description);
    }
}
