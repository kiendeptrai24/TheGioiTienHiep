using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class RandomSpawnClient : NetworkBehaviour
{
    [Header("Spawn Area")]
    [SerializeField] private Vector3 center = Vector3.zero;
    [SerializeField] private Vector3 size = new Vector3(200f, 5f, 200f);
    [SerializeField] private NetworkObject spawnObject;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsServer) return;
        RandomSpawn(20);
    }

    private void RandomSpawn(int value)
    {
        StartCoroutine(RandomSpawnCoroutine(value));
    }

    private IEnumerator RandomSpawnCoroutine(int value)
    {
        for (int i = 0; i < value; i++)
        {
            SpawnObject();

            yield return new WaitForSeconds(.2f);
        }
    }

    private void SpawnObject()
    {
        Vector3 randomPosition = GetRandomPosition();

        var netObj = Instantiate(spawnObject, randomPosition, Quaternion.identity);

        netObj.Spawn();
    }

    private Vector3 GetRandomPosition()
    {
        float randomX = Random.Range(500, 400);
        float randomZ = Random.Range(500, 400);

        return new Vector3(
            center.x + randomX,
            0,
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