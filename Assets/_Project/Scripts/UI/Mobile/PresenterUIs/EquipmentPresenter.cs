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
