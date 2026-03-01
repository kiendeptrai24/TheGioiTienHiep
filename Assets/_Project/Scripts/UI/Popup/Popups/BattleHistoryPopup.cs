using System;
using JetBrains.Annotations;
using UnityEngine;

public class BattleHistoryPopup : BasePopup<BattleHistoryDataPopup, BasePopupData>
{
    [SerializeField] private UIitemBattleHistory itemItemHistory;
    [SerializeField] private Transform content;
    public override void Show()
    {
        base.Show();
        //PopupAnimation.ShowPopup(rect, group, 0.5f);
    }
    protected override void SetupButtons()
    {
        base.SetupButtons();
    }
    public override void Hide()
    {
        //PopupAnimation.HidePopup(rect, group, 0.5f);
        base.Hide();
    }
    public void ShowPopup(BattleHistoryDataPopup data, Action<BasePopupData> onConfirm = null, Action onCancel = null)
    {
        this.onConfirm = onConfirm;
        this.onCancel = onCancel;
        ResetItem();
        SetupPopupData(data);
        PopupManager.Instance.ShowPopup<BattleHistoryPopup>(this);
    }
    protected override BasePopupData GetResult()
    {
        return null;
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
    }

    protected override void SetupPopupData(BattleHistoryDataPopup data)
    {
        foreach (var battleHistory in data.battleHistories)
        {
            UIitemBattleHistory item = Instantiate(itemItemHistory, content);
            item.ShowInfoUI(battleHistory);
        }
    }
    private void ResetItem()
    {
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            Destroy(content.GetChild(i).gameObject);
        }
    }
}