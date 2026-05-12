

using System.Collections.Generic;
using UnityEngine;
using TGTH.Mobile;

public class SkillPageManager : TGTHMonoBehaviour
{
    [SerializeField] private SkillPresenter presenter;
    [SerializeField] private List<InventoryItem> listItemDatas;
    private List<InventoryItem> listItemUsed = new List<InventoryItem>();
    public bool isAwake = false;
    protected override void Awake()
    {
        base.Awake();
        isAwake = true;
        UnLockTechnique();
    }
    public void SetInventoryData(List<InventoryItem> items)
    {
        listItemDatas = items;
        listItemDatas.AddRange(listItemUsed);
    }
    private void OnEnable()
    {
        presenter?.Refesh();
    }
    public bool AddItemData(ItemData data)
    {
        if (data is SkillData)
        {
            if (isAwake)
            {
                listItemDatas.Add(new InventoryItem(data));
            }
            else
            {
                listItemUsed.Add(new InventoryItem(data));
            }
            return true;
        }
        return false;
    }
    private void UnLockTechnique()
    {
        presenter.UnlockItem(GetItemUnlock());
    }
    public int GetItemUnlock()
    {
        int itemUnlockCount = 1;
        var cham = InventoryCenterManager.Instance.playerCham;
        if (cham == null) return itemUnlockCount;
        RealmType realmType = cham.realmType;

        switch (realmType)
        {
            case RealmType.LuyenKhi_1:
            case RealmType.LuyenKhi_2:
            case RealmType.LuyenKhi_3:
            case RealmType.LuyenKhi_4:
            case RealmType.LuyenKhi_5:
            case RealmType.LuyenKhi_6:
            case RealmType.LuyenKhi_7:
            case RealmType.LuyenKhi_8:
            case RealmType.LuyenKhi_9:
                itemUnlockCount = 1;
                break;
            case RealmType.TrucCo_SK:
            case RealmType.TrucCo_TK:
            case RealmType.TrucCo_HK:
            case RealmType.TrucCo_DVM:
                itemUnlockCount = 2;
                break;
        }
        return itemUnlockCount;
    }

}