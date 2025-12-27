using System;
using UnityEngine;
namespace TGTH.PC
{
    public class EquipmentPresenter : MonoBehaviour
    {
        private EquipmentSystem equipmentSystem;
        [SerializeField] private EquitmentPageView view;
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
                eq.OnItemClicked += HandleItemClicked;
            }
        }

        private void HandleItemClicked(UIItemSlotBase @base)
        {
            view.SetItemDescription(@base.inventoryItem);
        }

        public void SetEquipmentSystem(EquipmentSystem system)
        {
            equipmentSystem = system;
        }
        private void HandleEquippedChanged(InventoryItem oldItem, InventoryItem newItem)
        {
            view.SetItemDescription(newItem);
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
    
}
