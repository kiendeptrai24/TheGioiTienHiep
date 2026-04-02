
using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
public class PlayerProfile : TGTHNetworkBehaviour
{
    [SerializeField]
    private NetworkVariable<FixedString64Bytes> playerId = new(
        "",
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    private NetworkVariable<int> point = new(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    private void OnProfileReady(ProfileUser user)
    {
        OnLoadPlayerIdServerRpc(user.userId);
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        ProfileManager.Instance.OnProfileReady += OnProfileReady;
        string playerId = ProfileManager.Instance.GetProfile().userId;
        OnLoadPlayerIdServerRpc(playerId);
    }
    public FixedString64Bytes GetPlayerId() => playerId.Value;

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void OnLoadPlayerIdServerRpc(string playerId)
    {
        if (!IsServer) return;
        this.playerId.Value = new FixedString64Bytes(playerId);
    }

}