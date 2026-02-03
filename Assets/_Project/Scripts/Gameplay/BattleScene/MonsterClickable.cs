using UnityEngine;
using Unity.Netcode;

public class MonsterClickable : NetworkBehaviour, IWorldClickable
{
    public ulong MonsterNetId => NetworkObjectId;
    public void OnClicked()
    {
        MonsterOptionUI.Instance.Show(this);
    }
}
