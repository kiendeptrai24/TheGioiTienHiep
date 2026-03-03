using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattlePlaybackUI : TGTHMonoBehaviour
{
    [SerializeField] private Button exitBtn;
    [SerializeField] private Button startBtn;
    [SerializeField] private Toggle pauseToggle;
    [SerializeField] private TextMeshProUGUI pauseTxt;
    [SerializeField] private Button x2Btn;
    [SerializeField] private Button x3Btn;
    [SerializeField] private GameObject root;
    private BattlePlayback battlePlayback;

    protected override void Awake()
    {
        base.Awake();
        battlePlayback = BattlePlayback.Instance;
        BattlePlaybackManager.Instance.OnReadyGame += OnReadyGame;
        BattlePlaybackManager.Instance.OnEndGame += OnEndGame;
        exitBtn.onClick.AddListener(OnExitClicked);
        startBtn.onClick.AddListener(OnStartClicked);
        pauseToggle.onValueChanged.AddListener(OnPauseToggled);
        x2Btn.onClick.AddListener(OnX2Clicked);
        x3Btn.onClick.AddListener(OnX3Clicked);
        
    }

    private void OnEndGame()
    {
        battlePlayback.SetBattleTimer(1);
    }

    private void OnPauseToggled(bool isPause)
    {
        if (isPause)
            pauseTxt.text = "Pause";
        else
            pauseTxt.text = "Resume";
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

    private void OnReadyGame()
    {
        startBtn.gameObject.SetActive(true);
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

    private void OnExitClicked()
    {
        battlePlayback.StopBattle();
    }
}
