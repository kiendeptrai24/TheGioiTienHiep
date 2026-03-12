
using System;
using UnityEngine;

public class BattlePlaybackManager : Singleton<BattlePlaybackManager>
{
    private GameState state;
    public event Action OnStartGame;
    public event Action OnReadyGame;
    public event Action OnResultGame;
    public event Action OnEndGame;
    public event Action OnGameWin;
    public event Action OnGameLose;
    private BattlePlayback battlePlayback;
    protected override void Awake()
    {
        base.Awake();
        battlePlayback = GetComponent<BattlePlayback>();
        battlePlayback.OnEndBattle += OnEndBattle;
        battlePlayback.OnResultGame += OnResultBattle;
    }
    private void OnEndBattle()
    {
        OnEndGame?.Invoke();
    }
    private void OnResultBattle()
    {
        OnResultGame?.Invoke();
    }

    public void InitBattle()
    {
        ReadyGame();
    }

    public void StartGame()
    {
        state = GameState.StartGame;
        battlePlayback.StartBattle();
        OnStartGame?.Invoke();
    }

    public void ReadyGame()
    {
        state = GameState.ReadyGame;
        OnReadyGame?.Invoke();
    }

}