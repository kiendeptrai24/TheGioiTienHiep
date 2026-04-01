using System;
using Unity.Netcode;

public interface ISpawnable
{
    public void RemoveNetObject(NetworkObject entityObject);
    public void AddNetObject(NetworkObject entityObject);

}