using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;

public class MineOwnershipSystem : IOwnerShip
{
    public NetworkObject Owner { get; private set; }

    private readonly MineNetworkState networkState;

    public MineOwnershipSystem(
        MineNetworkState networkState)
    {
        this.networkState = networkState;
    }

    public bool HasOwner()
    {
        return string.IsNullOrEmpty(networkState.playerId) == false;
    }

    public bool IsOnline()
    {
        return Owner != null && HasOwner();
    }

    public bool IsOwner(string id)
    {
        return networkState.playerId == id;
    }

    public void SetOwner(
        NetworkObject owner,
        string playerId,
        double now)
    {
        Owner = owner;
        networkState.playerId = playerId;
    }

    public void ClearOwner()
    {
        Owner = null;
        networkState.playerId = "";
    }
}