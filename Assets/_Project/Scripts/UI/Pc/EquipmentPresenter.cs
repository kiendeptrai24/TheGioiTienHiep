using System;
using UnityEngine;
namespace TGTH.PC
{
    public class EquipmentPresenter : EquipmentBasePagePresenter
    {
        [SerializeField] private Transform equipmentContent;
        [SerializeField] private UIEquipmentSlot[] uiSlots;

        protected override void Awake()
        {
            base.Awake();
            uiSlots = equipmentContent.GetComponentsInChildren<UIEquipmentSlot>();
        }
    }
    
}
