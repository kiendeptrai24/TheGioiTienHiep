using System;
using Unity.Collections;
using Unity.Netcode;

public interface IOwnerShip
{
    public void SetOwner(
        NetworkObject owner,
        string playerId,
        double now);
    public bool HasOwner();
    public bool IsOnline();
    public bool IsOwner(string id);
    public void ClearOwner();
}