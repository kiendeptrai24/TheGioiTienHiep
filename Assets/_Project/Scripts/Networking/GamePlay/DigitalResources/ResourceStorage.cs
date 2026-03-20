using System;
using ExitGames.Client.Photon.StructWrapping;
using NUnit.Framework;
using Unity.Netcode;
using UnityEngine;

public class ResourceStorage : TGTHNetworkBehaviour
{

    public NetworkVariable<int> SpiritStone = new(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    public event Action<int> OnSpiritStoneChanged;

    private void OnCoinsChanged(ProfileUser user)
    {
        if (!IsServer) return;
        SpiritStone.Value = (int)user.coins;
    }
    public override void OnNetworkSpawn()
    {
        SpiritStone.OnValueChanged += HandleSpiritStoneChanged;
        OnCoinsChanged(ProfileManager.Instance.GetProfile());
    }

    private void HandleSpiritStoneChanged(int oldValue, int newValue)
    {
        OnSpiritStoneChanged?.Invoke(newValue);
    }

    public bool HasEnough(int amount)
    {
        return SpiritStone.Value >= amount;
    }

    // ===== SERVER ONLY =====
    public void Add(int amount)
    {
        if (!IsServer) return;
        SpiritStone.Value += amount;
    }

    public void Remove(int amount)
    {
        if (!IsServer) return;
        SpiritStone.Value -= amount;
    }
}
