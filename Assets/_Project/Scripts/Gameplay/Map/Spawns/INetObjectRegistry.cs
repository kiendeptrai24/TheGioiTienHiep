using System;
using Unity.Netcode;

public interface INetObjectRegistry
{
    public void RemoveNetObject(NetworkObject entityObject);
    public void AddNetObject(NetworkObject entityObject);

}