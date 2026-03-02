using UnityEngine;

public class PlayerTrigger : TGTHMonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        var entity = other.gameObject.GetComponent<EntityClickable>();
        if (entity == null) return;
        PlayerChoseObject.Instance.SetupEntity(entity);
    }
}
