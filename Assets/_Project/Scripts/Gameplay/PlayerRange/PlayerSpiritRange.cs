using UnityEngine;

public class PlayerSpiritRange : TGTHNetworkBehaviour
{
    [SerializeField] private SphereCollider collideCheckSpiritRange;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        var stat = GetComponent<StatsData>();
        collideCheckSpiritRange.radius = stat.SpiritRange;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<ItemMapWorld>(out var itemMapWorld))
        {
            itemMapWorld.ShowIcon();
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<ItemMapWorld>(out var itemMapWorld))
        {
            itemMapWorld.HideIcon();
        }
    }
}
