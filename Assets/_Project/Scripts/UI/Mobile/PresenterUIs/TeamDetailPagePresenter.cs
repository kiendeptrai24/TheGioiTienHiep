


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
    [SerializeField] private Dictionary<ItemData, Vector2Int> listAddDatas = new();
    private UIItemSlotBase currentItemSelect;
    private int currentlyDraggedItemIndex = -1;
    private bool isDraging;
    private ChampionListSnapshot championLS;
    protected override void Awake()
    {
        base.Awake();
        listAddDatas = new();
        view.OnCancelClicked += OnCancelClicked;
        view.OnOkClicked += OnOkClicked;
        inventoryCenterManager = InventoryCenterManager.Instance;
        championLS = ChampionListSnapshot.Instance;
        championLS.OnLoadDataSuccessed += () =>
        {
            SetInit(championLS.GetDicDatasChampionInTeam());
        };
        championLS.OnDataSave += () =>
        {
            SetInit(championLS.GetDicDatasChampionInTeam());
        };
        championLS.OnDataUndo += () =>
        {
            SetInit(championLS.GetDicDatasChampionInTeam());
        };
        InitializeInventoryUI();
        SetInit(championLS.GetDicDatasChampionInTeam());
    }

    private void OnOkClicked()
    {
        championLS.Save();
    }

    private void OnCancelClicked()
    {
        championLS.Undo();
    }
    public void SetInit(Dictionary<ItemData, Vector2Int> itemDatas)
    {
        listAddDatas.Clear();
        view.Reset();
        int index = 0;
        foreach (var item in itemDatas)
        {
            if (index >= inventoryCenterManager.MaxChampion())
                break;
            AddItemUI(item.Key, item.Value);
            index++;
        }
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

    public void AddItem(ItemData data, Vector2Int index)
    {
        if (listAddDatas.Count >= inventoryCenterManager.MaxChampion())
        {
            Debug.Log("Max Champion");
            return;
        }
        listAddDatas.Add(data, new Vector2Int(index.x, index.y));
        championLS.EquipData(data, index);
    }
    public void RemoveItem(ItemData data)
    {
        listAddDatas.Remove(data);
        championLS.UnEquipData(data);
    }
    private void HandleEmptySlotClicked(UIChoseChampionItem item)
    {
        chooseHeroPresenter.ChooseItem(item);
        Navigation(item);
    }
    public bool CheckTeamIsFull()
    {
        return listAddDatas.Count >= inventoryCenterManager.MaxChampion();
    }
    private void HandleEndDrag(UIItemSlotBase uiItem)
    {
        isDraging = false;
        ResetDrag();
    }
    public bool AddItemUI(ItemData itemData, Vector2 index)
    {
        if (listAddDatas.Count > inventoryCenterManager.MaxChampion())
        {
            return false;
        }

        ShowItem(itemData, index);
        int x = (int)index.x;
        int y = (int)index.y;
        var championIndex = new Vector2Int(x, y);
        AddItem(itemData, championIndex);
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
        AddItemUI(itemData, index);
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
            if (listAddDatas.ContainsKey(fromHero))
            {
                listAddDatas[fromHero] = toItem.championIndex;
                championLS.SwapIndex(fromHero, toItem.championIndex);
            }
        }

        if (toHero != null)
        {
            if (listAddDatas.ContainsKey(toHero))
            {
                listAddDatas[toHero] = fromItem.championIndex;
                championLS.SwapIndex(toHero, fromItem.championIndex);
            }
        }
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