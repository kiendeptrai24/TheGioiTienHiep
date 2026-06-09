using UnityEngine;
using Unity.Netcode;

public class RandomSpawnClient : NetworkBehaviour
{
    [Header("Spawn Area")]
    [SerializeField] private Vector3 center = Vector3.zero;
    [SerializeField] private Vector3 size = new Vector3(5f, 5f, 5f);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // Chỉ xử lý player của local client
        if (!IsOwner) return;

        RandomSpawn();
    }

    private void RandomSpawn()
    {
        Vector3 randomPosition = GetRandomPosition();

        // Nếu có NavMesh thì có thể sample thêm
        transform.position = randomPosition;
    }

    private Vector3 GetRandomPosition()
    {
        float randomX = Random.Range(-size.x / 2f, size.x / 2f);
        float randomT = Random.Range(-size.y / 2f, size.y / 2f);
        float randomZ = Random.Range(-size.z / 2f, size.z / 2f);

        return new Vector3(
            center.x + randomX,
            center.y + randomT,
            center.z + randomZ
        );
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(center, size);
    }
#endif
}