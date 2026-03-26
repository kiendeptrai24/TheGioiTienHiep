using UnityEngine;

public class PlayerTrigger : TGTHNetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!IsOwner) return;
        var entity = other.gameObject.GetComponent<EntityClickable>();
        if (entity.entityWorldType == EntityWorldType.Player) return;
        if (entity == null) return;
        PlayerChoseObject.Instance.SetupEntity(entity);
    }
}
