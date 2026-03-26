using UnityEngine;

public class PlayerTrigger : TGTHNetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!IsOwner) return;
        var entity = other.gameObject.GetComponent<EntityClickable>();
        if (entity == null) return;
        if (entity.entityWorldType == EntityWorldType.Player) return;
        PlayerChoseObject.Instance.SetupEntity(entity);
    }
}
