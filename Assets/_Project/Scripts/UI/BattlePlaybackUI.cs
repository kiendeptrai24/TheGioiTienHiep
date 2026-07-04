using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattlePlaybackUI : TGTHMonoBehaviour
{
    [SerializeField] private Button exitBtn;
    [SerializeField] private Button startBtn;
    [SerializeField] private Button endGameBtn;
    [SerializeField] private Toggle pauseToggle;
    [SerializeField] private TextMeshProUGUI pauseTxt;
    [SerializeField] private Button x2Btn;
    [SerializeField] private Button x3Btn;
    private BattlePlayback battlePlayback;

    protected override void Awake()
    {
        base.Awake();
        battlePlayback = BattlePlayback.Instance;
        BattlePlaybackManager.Instance.OnResultGame += OnBattleResult;
        BattlePlaybackManager.Instance.OnEndGame += OnEndGame;
        exitBtn.onClick.AddListener(OnExitClicked);
        startBtn.onClick.AddListener(OnStartClicked);
        endGameBtn.onClick.AddListener(OnExitClicked);
        pauseToggle.onValueChanged.AddListener(OnPauseToggled);
        x2Btn.onClick.AddListener(OnX2Clicked);
        x3Btn.onClick.AddListener(OnX3Clicked);
    }
    private void OnEnable()
    {
        Show();
    }
    private void OnEndGame()
    {
        battlePlayback.SetBattleTimer(1);
    }

    private void OnBattleResult()
    {
        Hide();
    }

    private void OnPauseToggled(bool isPause)
    {
        if (isPause)
            pauseTxt.text = "Dừng";
        else
            pauseTxt.text = "Tiếp tục";
        OnPauseClicked(isPause);
    }

    private void OnX2Clicked()
    {
        battlePlayback.SetBattleTimer(2);
    }

    private void OnX3Clicked()
    {
        battlePlayback.SetBattleTimer(3);
    }

    private void OnPauseClicked(bool isPause)
    {
        if (isPause)
            battlePlayback.SetBattleTimer(0);
        else
            battlePlayback.SetBattleTimer(1);
    }

    private void OnStartClicked()
    {
        BattlePlaybackManager.Instance.StartGame();
        startBtn.gameObject.SetActive(false);
    }
    private void Hide()
    {
        exitBtn.gameObject.SetActive(false);
        startBtn.gameObject.SetActive(false);
        endGameBtn.gameObject.SetActive(true);
        pauseToggle.gameObject.SetActive(false);
        pauseTxt.gameObject.SetActive(false);
        x2Btn.gameObject.SetActive(false);
        x3Btn.gameObject.SetActive(false);
    }
    private void Show()
    {
        exitBtn.gameObject.SetActive(true);
        startBtn.gameObject.SetActive(true);
        endGameBtn.gameObject.SetActive(false);
        pauseToggle.gameObject.SetActive(true);
        pauseTxt.gameObject.SetActive(true);
        x2Btn.gameObject.SetActive(true);
        x3Btn.gameObject.SetActive(true);
    }
    private void OnExitClicked()
    {
        battlePlayback.StopBattle();
    }
}
