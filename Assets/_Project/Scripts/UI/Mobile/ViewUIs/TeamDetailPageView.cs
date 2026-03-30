using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TeamDetailPageView : TGTHMonoBehaviour
{
    [Header("Content")]
    public Vector2 teamRatio;
    [SerializeField] private TextMeshProUGUI itemNameTxt;
    [SerializeField] private TextMeshProUGUI realmTxt;
    [SerializeField] private TextMeshProUGUI qualityTypeTxt;
    [SerializeField] private TextMeshProUGUI championIndexTxt;
    [SerializeField] private Image itemIconImge;
    [SerializeField] private Transform content;
    public MouseFollower mouseFollower;
    public List<UIItemSlotBase> listOfUIItems = new List<UIItemSlotBase>();
    protected override void Awake()
    {
        base.Awake();
        Init();
    }
    private void Init()
    {
        int rows = 4;
        int colsA = (int)teamRatio.x;
        int colsB = (int)teamRatio.y;

        int index = 0;
        // row = 0 là hàng dưới cùng (từ dưới lên)
        for (int row = 0; row < rows && index < listOfUIItems.Count; row++)
        {
            int colsThisRow = (row % 2 == 0) ? colsA : colsB;

            // trái -> phải
            for (int col = 0; col < colsThisRow && index < listOfUIItems.Count; col++)
            {
                var item = listOfUIItems[index] as UIChoseChampionItem;
                item.championIndex = new Vector2Int(col, rows - 1 - row); // (x=col, y=row)
                index++;
            }
        }
    }
    public void SetFollowerData(Sprite sprite, int quantity)
    {
        mouseFollower.SetData(sprite, quantity);
    }
    public void ToggleMouseFollower(bool enable)
    {
        mouseFollower.Toggle(enable);
    }

    public void DeselectItem(UIItemSlotBase uiItem)
    {
        if (uiItem)
        {
            uiItem.Deselect();
            uiItem = null;
        }
    }
    public void SelectUIItem(UIItemSlotBase uiItemOld, UIItemSlotBase uiItemNew)
    {
        if (uiItemOld != null)
            uiItemOld.Deselect();
        uiItemOld = uiItemNew;
        uiItemOld.Select();
    }
    public void ShowItemSelected(ItemData itemData)
    {
        var champion = itemData as HeroData;
        itemNameTxt.text = champion.itemName;
        realmTxt.text = EnumTranslator.ToVietnamese(champion.realmType);
        qualityTypeTxt.text = EnumTranslator.ToVietnamese(champion.qualityType);
        itemIconImge.sprite = champion.itemIcon;
        championIndexTxt.text = $"Vị trí Hiện tại: {champion.championIndex.x},{champion.championIndex.y}";

    }
}
