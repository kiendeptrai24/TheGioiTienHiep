

using System.Collections.Generic;
using UnityEngine;
using TGTH.Mobile;

public class TechniqueManager : TGTHMonoBehaviour
{
    [SerializeField] private TechniquePresenter presenter;
    [SerializeField] private List<InventoryItem> listItemDatas;
    [SerializeField] private List<InventoryItem> listItemUsed = new List<InventoryItem>();
    [SerializeField] private CharacterIdentity characterIdentity;
    public bool isAwake = false;
    protected override void Awake()
    {
        base.Awake();
        isAwake = true;
    }
    protected override void Start() {
        UnLockTechnique();
    }
    public void SetInventoryData(List<InventoryItem> items)
    {
        listItemDatas = items;
        listItemDatas.AddRange(listItemUsed);
        presenter?.ShowAllItems();
    }
    private void OnEnable()
    {
        presenter?.ShowAllItems();
    }
    public bool AddItemData(ItemData data)
    {
        if (data is TechniqueData)
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
    private void UnLockTechnique()
    {
        presenter.UnlockItem(2);
    }
}