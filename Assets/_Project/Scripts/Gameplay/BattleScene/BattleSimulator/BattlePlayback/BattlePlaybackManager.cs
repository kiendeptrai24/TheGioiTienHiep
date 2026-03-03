
using System;
using UnityEngine;
  
public class BattlePlaybackManager : TGTHMonoBehaviour
{
    private GameState state;
    public event Action OnStartGame;
    public event Action OnReadyGame;
    public event Action OnEndGame;

    public event Action OnGameWin;
    public event Action OnGameLose;
    private BattlePlayback battlePlayback;
    protected override void Awake()
    {
        base.Awake();
        battlePlayback = GetComponent<BattlePlayback>();
        battlePlayback.OnEndBattle += OnEndBattle;
    }
    private void OnEndBattle()
    {
        OnEndGame?.Invoke();
    }

    [ContextMenu("Test Init Battle")]
    public void InitBattle()
    {
        OnReadyGame?.Invoke();
        state = GameState.ReadyGame;
        ReadyGame();
        state = GameState.StartGame;
        OnStartGame?.Invoke();
        StartGame();
    }

    private void StartGame()
    {
        battlePlayback.StartBattle();
    }

    private void ReadyGame()
    {
        
    }

}