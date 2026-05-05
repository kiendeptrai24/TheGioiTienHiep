using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
namespace TGTH.Mobile
{
    /// <summary>
    /// this class must disable when start
    /// </summary>
    public class HeroEquipmentPresenter : EquipmentBasePagePresenter, IEndDragHandler
    {
        protected override bool HandleEquippedChanged(InventoryItem item1, InventoryItem item2)
        {
            if (base.HandleEquippedChanged(item1, item2))
            {
                var heroData = statsManager.heroData as HeroData;
                if (item1 != null && item1.data != null)
                {
                    heroData.equipmentDatas.Remove(item1.data as EquipmentData);
                }
                if (item2 != null && item2.data != null)
                {
                    heroData.equipmentDatas.Add(item2.data as EquipmentData);
                }
                return true;
            }
            return false;
        }
    }
}
