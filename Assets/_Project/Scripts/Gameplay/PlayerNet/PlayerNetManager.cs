

using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetManager : Singleton<PlayerNetManager>
{
    [SerializeField] private NetworkObject playerObject;
    public event Action<NetworkObject> OnPlayerExists;
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
    }
    public void SetPlayerObject(NetworkObject _playerObject)
    {
        playerObject = _playerObject;
        OnPlayerExists?.Invoke(playerObject);
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
    }
}