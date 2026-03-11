using Unity.Netcode;
using UnityEngine;

public abstract class EntityClickable : NetworkBehaviour, IWorldClickable
{
    public ulong EntityNetId => NetworkObjectId;
    public void OnClicked()
    {
        PlayerChoseObject.Instance.SetupEntity(this);
        Debug.Log("Clicked" + gameObject.name);
    }
    public abstract void OnEntityClickedAccept(NetworkObject network);
}
