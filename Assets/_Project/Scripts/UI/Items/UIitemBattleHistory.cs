using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIitemBattleHistory : TGTHMonoBehaviour
{
    [SerializeField] private Button startShowBtn;
    [SerializeField] private TextMeshProUGUI titleTxt;
    [SerializeField] private TextMeshProUGUI descriptionTxt;
    private List<BattleEvent> battleEvents;
    protected override void Awake()
    {
        base.Awake();
        startShowBtn.onClick.AddListener(OnStartShowClicked);
    }
    private void OnStartShowClicked()
    {
        BattlePlayback.Instance.SetBattleEvents(battleEvents);
        CameraSwitchManager.Instance.SwitchToBattle();
        PopupManager.Instance.HideAllPopups();
    }
    public void ShowInfoUI(BattleHistory battleHistory)
    {
        titleTxt.text = battleHistory.name;
        descriptionTxt.text = battleHistory.dateTime.ToString() + "\n" + battleHistory.winner;
        battleEvents = battleHistory.battleEvents;
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
    }
}
