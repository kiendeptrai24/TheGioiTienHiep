
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
    private InventoryCenterManager inventoryCenterManager;
    private void OnProfileReady(ProfileUser user)
    {
        OnLoadPlayerIdServerRpc(user.userId);
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        inventoryCenterManager = InventoryCenterManager.Instance;
        inventoryCenterManager.OnItemPlayerChanged += OnItemPlayerChanged;
        OnItemPlayerChanged(inventoryCenterManager.playerCham);
        ProfileManager.Instance.OnProfileReady += OnProfileReady;
        string playerId = ProfileManager.Instance.GetProfile().userId;
        OnLoadPlayerIdServerRpc(playerId);
    }

    private void OnItemPlayerChanged(ItemData data)
    {
        if (data == null) return;
        GetComponent<StatsData>().SetUpItem(data);
    }

    public FixedString64Bytes GetPlayerId() => playerId.Value;

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void OnLoadPlayerIdServerRpc(string playerId)
    {
        if (!IsServer) return;
        this.playerId.Value = new FixedString64Bytes(playerId);
    }

}