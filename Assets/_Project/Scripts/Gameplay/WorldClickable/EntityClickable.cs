using Unity.Netcode;
using UnityEngine;

public abstract class EntityClickable : NetworkBehaviour, IWorldClickable
{
    public ulong EntityNetId => NetworkObjectId;
    public void OnClicked()
    {
        MonsterOptionUI.Instance.Show(this);
        Debug.Log("Entity Clicked: " + EntityNetId);
    }
    public abstract void OnEntityClickedAccept(NetworkObject network);
}
