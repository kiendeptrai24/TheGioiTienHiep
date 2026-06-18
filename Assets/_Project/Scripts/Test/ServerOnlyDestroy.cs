using Unity.Netcode;
using UnityEngine;

public class ServerOnlyDestroy : TGTHMonoBehaviour
{
    protected override void Start()
    {
        base.Start();
        if (Configuration.Instance.IsServerBuild())
        {
            Destroy(gameObject);
        }
    }

}