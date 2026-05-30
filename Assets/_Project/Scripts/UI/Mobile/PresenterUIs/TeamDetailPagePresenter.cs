


using System;
using System.Collections.Generic;
using TGTH.Mobile;
using UnityEngine;

public class TeamDetailPagePresenter : TGTHMonoBehaviour
{
    private class ChampionData
    {
        public InventoryItem item;
        public Vector2 championIndex;
    }
    [SerializeField] private TeamDetailPageView view;
    [SerializeField] private ChooseHeroPresenter chooseHeroPresenter;
    private InventoryCenterManager inventoryCenterManager;
    [SerializeField] private List<ItemData> listDatas = new();
    private UIItemSlotBase currentItemSelect;
    private int currentlyDraggedItemIndex = -1;
    private bool isDraging;
    public int maxChampion = 4;

    protected override void Awake()
    {
        base.Awake();
        inventoryCenterManager = InventoryCenterManager.Instance;
        inventoryCenterManager.OnLoadDataSuccessed += () =>
        {
            listDatas.Clear();
            view.Reset();
            SetInit(inventoryCenterManager.GetDatasChampionInTeam());
        };
        InitializeInventoryUI();
    }
    protected override void Start()
    {
        base.Start();
        SetInit(inventoryCenterManager.GetDatasChampionInTeam());
    }
    public void SetInit(List<ItemData> itemDatas)
    {
        int index = 0;
        foreach (var item in itemDatas)
        {
            if (index >= maxChampion)
                break;
            AddItem(item, (item as HeroData).championIndex);
            index++;
        }
    }
    public List<ItemData> GetAllItems()
    {
        var items = new List<ItemData>();
        foreach (var item in listDatas)
        {
            items.Add(item);
        }
        return items;
    }

    private void InitializeInventoryUI()
    {

        foreach (var uiItem in view.listOfUIItems)
        {
            var item = uiItem as UIChoseChampionItem;
            if (item == null)
                continue;
            item.OnItemClicked += HandleItemClicked;
            item.OnItemBeginDrag += HandleBeginDrag;
            item.OnItemDroppedOn += HandleItemDropped;
            item.OnItemEndDrag += HandleEndDrag;
            item.OnEmptySlotClicked += HandleEmptySlotClicked;
        }

    }

    public void AddItem(ItemData data)
    {
        if (listDatas.Count >= maxChampion)
        {
            Debug.Log("Max Champion");
            return;
        }
        listDatas.Add(data);
        inventoryCenterManager.EquipData(data);
        inventoryCenterManager.SetItemChampionData(GetAllItems());
    }
    public void RemoveItem(ItemData data)
    {
        listDatas.Remove(data);
        inventoryCenterManager.UnEquipData(data);
        inventoryCenterManager.SetItemChampionData(GetAllItems());
    }
    private void HandleEmptySlotClicked(UIChoseChampionItem item)
    {
        chooseHeroPresenter.ChooseItem(item);
        Navigation(item);
    }
    public bool CheckTeamIsFull()
    {
        return listDatas.Count >= maxChampion;
    }
    private void HandleEndDrag(UIItemSlotBase uiItem)
    {
        isDraging = false;
        ResetDrag();
    }
    public bool AddItem(ItemData itemData, Vector2 index)
    {
        if (listDatas.Count >= maxChampion)
        {
            return false;
        }

        ShowItem(itemData, index);
        int x = (int)index.x;
        int y = (int)index.y;
        (itemData as HeroData).championIndex = new Vector2Int(x, y);
        AddItem(itemData);
        return true;
    }

    private void ShowItem(ItemData itemData, Vector2 index)
    {
        for (int i = 0; i < view.listOfUIItems.Count; i++)
        {
            if (view.listOfUIItems[i].HasItem())
                continue;

            var itemChoseChampion = view.listOfUIItems[i] as UIChoseChampionItem;

            if (itemChoseChampion == null)
                continue;

            if (itemChoseChampion.championIndex != index)
                continue;

            view.listOfUIItems[i].SetItem(new InventoryItem(itemData));
            break;
        }
    }

    public void SwapItem(ItemData itemData, Vector2 index)
    {
        for (int i = 0; i < view.listOfUIItems.Count; i++)
        {
            var itemChoseChampion = view.listOfUIItems[i] as UIChoseChampionItem;

            if (itemChoseChampion == null)
                continue;

            if (itemChoseChampion.championIndex != index)
                continue;
            RemoveItem(view.listOfUIItems[i].inventoryItem.data);
            view.listOfUIItems[i].SetItem(new InventoryItem(itemData));
            break;
        }
        AddItem(itemData);
    }
    private void HandleItemDropped(UIItemSlotBase uiItem)
    {
        if (currentlyDraggedItemIndex == -1) return;
        int dropIndex = view.listOfUIItems.IndexOf(uiItem);

        if (dropIndex == -1) return;

        SwapItemsUI(currentlyDraggedItemIndex, dropIndex);
        ItemClicked(uiItem);
    }

    private void HandleBeginDrag(UIItemSlotBase uiItem)
    {
        isDraging = true;
        int index = view.listOfUIItems.IndexOf(uiItem);
        if (index < 0) return;

        currentlyDraggedItemIndex = index;

        var item = uiItem.inventoryItem;
        if (item == null) return;

        view.ToggleMouseFollower(true);
        view.SetFollowerData(item.data.itemIcon, item.data.currentstack);
        view.ShowItemSelected(uiItem.inventoryItem.data);
    }
    private void ItemClicked(UIItemSlotBase uiItem)
    {
        int index = view.listOfUIItems.IndexOf(uiItem);
        if (index < 0) return;

        view.DeselectItem(currentItemSelect);
        view.SelectUIItem(currentItemSelect, uiItem);

        currentItemSelect = uiItem;
        ResetDrag();
    }
    private void SwapItemsUI(int from, int to)
    {
        var fromSlot = view.listOfUIItems[from];
        var toSlot = view.listOfUIItems[to];
        if (fromSlot == toSlot)
        {
            ResetDrag();
            return;
        }
        ChangePosition(fromSlot, toSlot);
        fromSlot.SwapWith(toSlot);
    }
    public void ChangePosition(UIItemSlotBase from, UIItemSlotBase to)
    {
        var fromItem = from as UIChoseChampionItem;
        var toItem = to as UIChoseChampionItem;

        var fromHero = fromItem?.inventoryItem?.data as HeroData;
        var toHero = toItem?.inventoryItem?.data as HeroData;

        if (fromHero == null && toHero == null)
            return;

        if (fromHero != null)
        {
            fromHero.championIndex = toItem.championIndex;
        }

        if (toHero != null)
        {
            toHero.championIndex = fromItem.championIndex;
        }
        inventoryCenterManager.SetItemChampionData(GetAllItems());
    }
    private void HandleItemClicked(UIItemSlotBase uiItem)
    {
        if (isDraging)
        {
            isDraging = false;
            return;
        }
        var popup = PopupManager.Instance.GetPopup<UseItemPopup>();
        BaseSetupData data = new BaseSetupData($"Bạn có muốn loại bỏ tướng <color=green>{uiItem.inventoryItem.data.itemName}</color> khỏi đội không?");
        if (popup != null)
        {
            popup.ShowPopup(data,
            onConfirm: (BasePopupData result) =>
            {
                RemoveItem(uiItem.inventoryItem.data);
                uiItem.ResetData();
            },
            onCancel: () =>
            {

            },
            onInfo: () =>
            {
                ItemClicked(uiItem);
                Navigation(uiItem);
            });
        }
        view.ShowItemSelected(uiItem.inventoryItem.data);
    }
    private void Navigation(UIItemSlotBase uiItem)
    {
        uiItem?.navigation.OnClick();
    }
    private void ResetDrag()
    {
        view.ToggleMouseFollower(false);
        currentlyDraggedItemIndex = -1;
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
        view = GetComponent<TeamDetailPageView>();
    }
}