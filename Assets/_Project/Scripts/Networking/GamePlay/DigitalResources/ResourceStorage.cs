using System;
using Unity.Netcode;
using UnityEngine;

public class ResourceStorage : TGTHNetworkBehaviour
{
    public NetworkVariable<ulong> Coins = new(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    public event Action<ulong> OnCoinsChanged;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Coins.OnValueChanged += HandleCoinsChanged;
        if (!IsOwner) return;
        ulong coins = ProfileManager.Instance.GetProfile().coins;
        OnLoadCoinsServerRpc(coins);
    }
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    private void OnLoadCoinsServerRpc(ulong coins)
    {
        if (!IsServer) return;
        Coins.Value = coins;
    }
    private void HandleCoinsChanged(ulong oldValue, ulong newValue)
    {
        OnCoinsChanged?.Invoke(newValue);
    }

    public bool HasEnough(ulong amount)
    {
        if (!IsServer) return false;
        return Coins.Value >= amount;
    }

    // ===== SERVER ONLY =====
    public void PlusCost(ulong amount)
    {
        if (!IsServer) return;
        Coins.Value += amount;
    }

    public void MinusCost(ulong amount)
    {
        if (!IsServer) return;
        Coins.Value -= amount;
    }

    // ===== OFFLINE COINS =====
    public void AddOfflineCoins(ulong amount)
    {
        if (!IsServer) return;
        Coins.Value += amount;
    }
}
