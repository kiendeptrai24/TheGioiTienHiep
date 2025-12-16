using System;
using UnityEngine;

public class EquipmentPresenter : MonoBehaviour
{
    private EquipmentSystem equipmentSystem;
    [SerializeField] private Transform equipmentContent;
    [SerializeField] private UIEquipmentSlot[] uiSlots;

    private void Awake()
    {
        uiSlots = equipmentContent.GetComponentsInChildren<UIEquipmentSlot>();
    }
    private void Start() {
        foreach (var eq in uiSlots)
        {
            eq.OnEquippedChanged += HandleEquippedChanged;
        }
    }
    public void SetEquipmentSystem(EquipmentSystem system)
    {
        equipmentSystem = system;
    }
    private void HandleEquippedChanged(InventoryItem oldItem, InventoryItem newItem)
    {
        if(equipmentSystem == null) return;
        equipmentSystem.Unequip(oldItem);
        equipmentSystem.Equip(newItem);
    }
    private void OnDestroy() {
        foreach (var eq in uiSlots)
        {
            eq.OnEquippedChanged -= HandleEquippedChanged;
        }
    }
}
