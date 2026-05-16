using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class MineOwnershipSystem
{
    public NetworkObject Owner { get; private set; }

    public readonly List<MineOwnershipSegment> History =
        new();

    private readonly MineNetworkState networkState;

    public MineOwnershipSystem(
        MineNetworkState networkState)
    {
        this.networkState = networkState;
    }

    public bool HasOwner()
    {
        return networkState.PlayerId.Value.IsEmpty == false;
    }

    public bool IsOnline()
    {
        return Owner != null && HasOwner();
    }

    public bool IsOwner(FixedString64Bytes id)
    {
        return networkState.PlayerId.Value == id;
    }

    public void SetOwner(
        NetworkObject owner,
        string playerId,
        double now)
    {
        Owner = owner;

        networkState.PlayerId.Value = playerId;

        AddHistory(
            playerId,
            (float)now,
            -1
        );
    }

    public void ClearOwner()
    {
        Owner = null;
        networkState.PlayerId.Value = "";
    }

    public MineOwnershipSegment AddHistory(
        FixedString64Bytes playerId,
        float start,
        float end)
    {
        MineOwnershipSegment seg =
            new MineOwnershipSegment
            {
                OwnerId = playerId,
                StartTime = start,
                EndTime = end
            };

        History.Add(seg);

        return seg;
    }

    public MineOwnershipSegment GetSegment(
        FixedString64Bytes id)
    {
        foreach (var seg in History)
        {
            if (seg.OwnerId == id)
                return seg;
        }

        return null;
    }
}