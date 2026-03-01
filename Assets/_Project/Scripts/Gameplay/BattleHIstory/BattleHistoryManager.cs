using System;
using UnityEngine;

public class BattleHistoryManager : TGTHMonoBehaviour
{
    public float timeScale;
    public GameState gameState;
    #region Game Events
    public event Action OnReadyBattle;
    public event Action OnStartBattle;
    public event Action OnPauseBattle;
    public event Action OnResumeBattle;
    public event Action OnEndBattle;
    #endregion
    public void SpauseGame()
    {
        TimeScaleManager.SetUnityTimeScale(0);
        gameState = GameState.PauseGame;
        OnPauseBattle?.Invoke();
    }
    public void ResumeGame()
    {
        TimeScaleManager.SetUnityTimeScale(timeScale);
        gameState = GameState.ResumeGame;
        OnResumeBattle?.Invoke();
    }
    public void ReadyGame()
    {
        OnResumeBattle?.Invoke();
    }
    public void StartGame()
    {
        gameState = GameState.StartGame;
        OnStartBattle?.Invoke();
    }
    public void EndGame()
    {
        gameState = GameState.EndGame;
        OnEndBattle?.Invoke();
    }
}
