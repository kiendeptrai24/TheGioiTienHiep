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
        RaiseUnActive();
    }

    private void OnStartGame()
    {

    }

    private void OnReadyGame()
    {
        RaiseActive();
    }
}