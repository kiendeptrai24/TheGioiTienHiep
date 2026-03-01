
using UnityEngine;
using UnityEngine.UI;

public class ActionBattleHistoryButton : TGTHMonoBehaviour
{
    [SerializeField] private Button okeBtn;
    [SerializeField] private BattleHistoryController battleHistoryController;
    protected override void Awake()
    {
        base.Awake();
        okeBtn.onClick.AddListener(OnClickBtn);
    }

    private void OnClickBtn()
    {
        var popup = PopupManager.Instance.GetPopup<BattleHistoryPopup>();
        var data = new BattleHistoryDataPopup(battleHistoryController.battleEventsHistory);
        popup.ShowPopup(data);
    }

    protected override void Start()
    {
        base.Start();
        okeBtn = GetComponent<Button>();
    }
}