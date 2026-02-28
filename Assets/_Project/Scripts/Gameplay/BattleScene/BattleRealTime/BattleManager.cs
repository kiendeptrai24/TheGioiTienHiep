using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BattleManager : TGTHNetworkBehaviour
{
    private GameState state;
    public event Action OnStartGame;
    public event Action OnReadyGame;
    public event Action OnEndGame;

    public event Action OnGameWin;
    public event Action OnGameLose;
    private List<NetworkObject> heroes = new();
    private List<NetworkObject> enemies = new();
    [SerializeField] Transform center;
    private bool _ended = false;
    protected override void Start()
    {
        if (!IsServer) return;
        InitBattle();
    }
    private void InitBattle()
    {
        ReadyGame();
        state = GameState.StartGame;
        OnStartGame?.Invoke();
        StartGame();
    }
    [ContextMenu("Ready Game")]
    public void ReadyGame()
    {
        state = GameState.ReadyGame;
        OnReadyGame?.Invoke();
    }
    [ContextMenu("Start Game")]
    public void StartGame()
    {
        // foreach (var hero in heroes)
        // {
        //     hero.GetComponent<TargetFinderBase>().battleState = true;
        // }
        // foreach (var enemy in enemies)
        // {
        //     enemy.GetComponent<TargetFinderBase>().battleState = true;
        // }
        state = GameState.StartGame;
        OnStartGame?.Invoke();
    }
    public void SetListEnemy(List<NetworkObject> enemies) => this.enemies = enemies;
    public void SetListHero(List<NetworkObject> heroes) => this.heroes = heroes;
    private void ClearObject()
    {
        foreach (var hero in heroes)
        {
            hero.Despawn();
            Destroy(hero.gameObject);
        }
        foreach (var enemy in enemies)
        {
            enemy.Despawn();
            Destroy(enemy.gameObject);
        }

    }
    private void Update()
    {
        if (!IsServer) return;
        if (_ended) return;
        if (state != GameState.StartGame) return;

        heroes.RemoveAll(h => h == null);
        enemies.RemoveAll(e => e == null);

        if (heroes.Count == 0)
        {
            _ended = true;
            OnBattleLose();
            return;
        }

        if (enemies.Count == 0)
        {
            _ended = true;
            OnBattleWin();
            return;
        }
    }
    [ContextMenu("Win")]
    public void OnBattleWin()
    {
        ClearObject();
        ReturnToOverworld();
        state = GameState.EndGame;
        OnGameWin?.Invoke();
        OnEndGame?.Invoke();
    }
    [ContextMenu("Lost")]
    public void OnBattleLose()
    {
        ClearObject();
        ReturnToOverworld();
        state = GameState.EndGame;
        OnEndGame?.Invoke();
        OnGameLose?.Invoke();
    }

    private void ReturnToOverworld()
    {
        Debug.Log("ReturnToOverworld");
    }
}
