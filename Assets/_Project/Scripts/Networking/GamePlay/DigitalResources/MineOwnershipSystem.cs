using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;

public class MineOwnershipSystem : IOwnerShip
{
    private MineNetworkState networkState;

    public MineOwnershipSystem(MineNetworkState networkState)
    {
        this.networkState = networkState;
    }

    public bool HasOwner()
    {
        return string.IsNullOrEmpty(networkState.playerId) == false;
    }

    public bool IsOnline()
    {
        return networkState.Owner != null && HasOwner();
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
        networkState.Owner = owner;
        networkState.playerId = playerId;
    }

    public void ClearOwner()
    {
        networkState.Owner = null;
        networkState.playerId = "";
    }
}