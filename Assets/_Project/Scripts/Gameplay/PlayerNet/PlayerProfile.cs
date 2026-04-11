
using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
public class PlayerProfile : TGTHNetworkBehaviour
{
    private ResourceStorage resourceStorage;
    [SerializeField]
    private NetworkVariable<FixedString64Bytes> playerId = new(
        "",
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    [SerializeField] private PlayerResource playerResource = new();
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        InventoryCenterManager inventoryCenterManager;
        var user = ProfileManager.Instance.GetProfile();
        if (IsOwner)
            playerResource = user.playerResource;

        resourceStorage = GetComponent<ResourceStorage>();
        resourceStorage.OnCoinsChanged += OnCoinsChanged;

        inventoryCenterManager = InventoryCenterManager.Instance;
        inventoryCenterManager.OnItemPlayerChanged += OnItemPlayerChanged;

        OnItemPlayerChanged(inventoryCenterManager.playerCham);
        OnLoadPlayerIdServerRpc(user.userId);
    }

    private void OnCoinsChanged(ulong obj)
    {
        playerResource.linhThach = (int)resourceStorage.Coins.Value;
    }

    private void OnItemPlayerChanged(ItemData data)
    {
        if (data == null) return;
        GetComponent<StatsData>().SetUpItem(data);
    }
    public PlayerResource GetPlayerResource()
    {
        return playerResource;
    }
    public FixedString64Bytes GetPlayerId() => playerId.Value;

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void OnLoadPlayerIdServerRpc(string playerId)
    {
        if (!IsServer) return;
        this.playerId.Value = new FixedString64Bytes(playerId);
    }

}