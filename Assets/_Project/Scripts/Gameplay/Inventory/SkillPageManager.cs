

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
    }
    public void SetInventoryData(List<InventoryItem> items)
    {
        listItemDatas = items;
        listItemDatas.AddRange(listItemUsed);
    }
    private void OnEnable() {
        presenter?.Refesh();
    }
    public bool AddItemData(ItemData data)
    {
        if (data is SkillData)
        {
            if(isAwake)
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
    
}