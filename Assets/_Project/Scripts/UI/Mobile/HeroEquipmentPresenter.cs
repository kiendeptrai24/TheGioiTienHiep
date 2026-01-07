using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
namespace TGTH.Mobile
{
    /// <summary>
    /// this class must disable when start
    /// </summary>
    public class HeroEquipmentPresenter : EquipmentBasePagePresenter, IEndDragHandler
    {
        [SerializeField] private StatsData statsManager;
        private bool setup = false;
        protected override void Awake()
        {
            base.Awake();
            Init();
            view.ShowEquipmentItems(statsManager.data);
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            // UpdateDataItem();
        }
        public void UpdateDataItem()
        {
            if (!setup)
            {
                Init();
                setup = true;
            }
        }
        private void Init()
        {
            view.equipmentSlotsDictionary = new Dictionary<EquipmentType, UIItemSlotBase>();
            foreach (var slot in view.listOfEquitmentItems)
            {
                view.equipmentSlotsDictionary.Add(slot.equipmentType, slot);
            }
        }
        protected override bool HandleEquippedChanged(InventoryItem item1, InventoryItem item2)
        {
            if (base.HandleEquippedChanged(item1, item2))
            {
                var heroData = statsManager.data as HeroData;
                if (item1 != null && item1.data != null)
                {
                    heroData.equitmentDatas.Remove(item1.data as EquitmentData);
                }
                if (item2 != null && item2.data != null)
                {
                    heroData.equitmentDatas.Add(item2.data as EquitmentData);
                }
                return true;
            }
            return false;
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
        public void HandleItemClicked(InventoryItem inventoryItem)
        {
            if (inventoryItem == null) return;
            if (inventoryItem.data is HeroData heroData)
            {
                view.ShowEquipmentItems(heroData);
            }
        }
    }
}
