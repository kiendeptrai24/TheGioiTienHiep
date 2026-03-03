using System;
using UnityEngine;

public class ActiveBattleStateNotifier : ActiveStateNotifier
{
    private BattlePlaybackManager battlePlaybackManager;
    protected override void Awake()
    {
        base.Awake();
        battlePlaybackManager = GetComponent<BattlePlaybackManager>();
        battlePlaybackManager.OnReadyGame += OnReadyGame;
        battlePlaybackManager.OnStartGame += OnStartGame;
        battlePlaybackManager.OnEndGame += OnEndGame;
    }

    private void OnEndGame()
    {
        Debug.Log("end game");
        RaiseUnActive();
    }

    private void OnStartGame()
    {

    }

    private void OnReadyGame()
    {
        Debug.Log("ready game");
        RaiseActive();
    }
}