using Unity.Netcode;
using UnityEngine;

public abstract class EntityClickable : NetworkBehaviour, IWorldClickable
{
    public ulong EntityNetId => NetworkObjectId;
    public EntityWorldType entityWorldType;
    public void OnClicked()
    {
        Debug.Log("dsadasdas");
        PlayerChoseObject.Instance.SetupEntity(this);
    }
    public abstract void OnEntityClickedAccept(NetworkObject network);
}
