using System.Collections.Generic;
using DuloGames.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDetailPageView : IItemDetailPageView
{
    [Header("Content")]
    [SerializeField] private TextMeshProUGUI itemNameTxt;
    [SerializeField] private TextMeshProUGUI realmTxt;
    [SerializeField] private TextMeshProUGUI qualityTypeTxt;
    [SerializeField] private Image itemIconImge;
    [SerializeField] private List<UIEquipmentSlot> uIEquipmentSlots;
    [SerializeField] private List<UIItemSlotBase> uISkillItems;
    [SerializeField] private List<UIItemSlotBase> uITechniqueItems;

    protected override void Awake()
    {
        base.Awake();
    }
    public override void HandleItemClicked(InventoryItem inventoryItem)
    {

    }
}
