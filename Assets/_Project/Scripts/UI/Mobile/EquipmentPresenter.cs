using System.Collections.Generic;
using UnityEngine.EventSystems;
namespace TGTH.Mobile
{
    public class EquipmentPresenter : EquipmentBasePagePresenter , IEndDragHandler
    {
        private void ShowAllItems()
        {
            view.ShowAllItems(listItemDatas);
        }
        private void ShowAllItemsInInventory()
        {
            var equip = GetListItemEquipment();
            List<InventoryItem> filteredList = new();

            foreach (var item in listItemDatas)
            {
                if (!equip.Contains(item.data))
                    filteredList.Add(item);
            }

            view.ShowAllItemInInventory(filteredList);
        }

        // private void SortInventory()
        // {
        //     // get equipqment type and quality in UI
        //     int type = view.eqipmenttypeDrop.value + 1;
        //     int quality = view.qualityTypeDrop.value;

        //     //convert to EquipmentType and QualityType
        //     EquipmentType selectedType = (EquipmentType)type;
        //     QualityType selectedQuality = (QualityType)quality;

        //     // get equipment item 
        //     var equip = GetListItemEquipment();

        //     // create list item dont have item is equipment
        //     List<InventoryItem> filteredList = new();
        //     foreach (var item in listItemDatas)
        //     {
        //         if (!equip.Contains(item.data))
        //             filteredList.Add(item);
        //     }

        //     // sort item base on EquipmentType and QualityType
        //     var sortedList = filteredList
        //         .Where(inv =>
        //         {
        //             var eq = (EquitmentData)inv.data;
        //             return (type == 0 || eq.equipmentType == selectedType)
        //                 && (quality == 0 || eq.qualityType == selectedQuality);
        //         })
        //         .OrderBy(inv => ((EquitmentData)inv.data).equipmentType)
        //         .ThenByDescending(inv => ((EquitmentData)inv.data).qualityType)
        //         .ToList();

        //     // if sortlist dont have item return empty list
        //     if (sortedList.Count == 0)
        //         sortedList = new();

        //     // show in ui
        //     view.ShowAllItemInInventory(sortedList);
        // }
        private HashSet<ItemData> GetListItemEquipment()
        {
            HashSet<ItemData> temp = new();
            foreach (var item in view.listOfEquitmentItems)
            {
                if (item.inventoryItem != null)
                {
                    temp.Add(item.inventoryItem.data);
                }
            }
            return temp;
        }
    }
}
