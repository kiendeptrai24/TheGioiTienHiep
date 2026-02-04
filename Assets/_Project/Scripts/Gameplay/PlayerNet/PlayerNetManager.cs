

using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetManager : Singleton<PlayerNetManager>
{
    [SerializeField] private NetworkObject playerObject;
    public event Action<NetworkObject> OnPlayerExiststed;
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
        SetPlayerProfile();
        OnPlayerExiststed?.Invoke(playerObject);
    }
    public void SetPlayerProfile()
    {
        var storage = playerObject.GetComponent<ResourceStorage>();
        ProfileManager.Instance.BindResource(storage);
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
    }
}