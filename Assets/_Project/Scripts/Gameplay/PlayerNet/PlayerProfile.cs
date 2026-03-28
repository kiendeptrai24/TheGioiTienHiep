
using System;
using Unity.Netcode;
using UnityEngine;
public class PlayerProfile : TGTHNetworkBehaviour
{
    [SerializeField] private string playerId;
    protected override void Awake()
    {
        base.Awake();
    }
    private void OnProfileReady(ProfileUser user)
    {
        playerId = user.userId;
        OnLoadPlayerIdServerRpc(playerId);

        Debug.Log("[PlayerProfile] OnProfileReady " + playerId);
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner) return;
        ProfileManager.Instance.OnProfileReady += OnProfileReady;
        playerId = ProfileManager.Instance.GetProfile().userId;
    }
    public string GetPlayerId() => playerId;

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    private void OnLoadPlayerIdServerRpc(string playerId)
    {
        if (!IsServer) return;
        this.playerId = playerId;
    }
}