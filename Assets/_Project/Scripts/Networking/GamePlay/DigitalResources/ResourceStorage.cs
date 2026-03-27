using System;
using Unity.Netcode;
using UnityEngine;

public class ResourceStorage : TGTHNetworkBehaviour, ISaveable
{
    public NetworkVariable<ulong> Coins = new(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    public event Action<ulong> OnCoinsChanged;

    public override void OnNetworkSpawn()
    {
        Coins.OnValueChanged += HandleCoinsChanged;
        HandleCoinsChanged(0, Coins.Value);
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
        Debug.Log($"[OfflineCoins] Added {amount} coins. Total: {Coins.Value}");
    }

    public void LoadData(GameData _data)
    {
        if (!IsServer) return;
        Coins.Value = _data.coins;
    }

    public void SaveGame(ref GameData _data)
    {
        if (!IsServer) return;
        _data.coins = (ulong)Coins.Value;
    }
}
