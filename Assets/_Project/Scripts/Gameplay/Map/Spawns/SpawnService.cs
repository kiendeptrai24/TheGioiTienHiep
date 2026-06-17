using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnService : TGTHNetworkBehaviour
{
    public void SpawnNetwork(List<GameObject> listPrefab, ISpawnArea area, ISpawnPattern pattern, SpawnSettings settings, Transform parent = null)
    {
        if (!IsServer) return;
        List<Vector3> points = pattern.GeneratePoints(area, settings);

        foreach (var point in points)
        {
            int index = Random.Range(0, listPrefab.Count);
            GameObject prefab = listPrefab[index];
            NetworkObjectPool.Singleton.GetNetworkObject(prefab, point, Quaternion.identity);
        }
    }
    public void ReturnToPool(NetworkObject entityNet)
    {
        if (!IsServer) return;
        NetworkObjectPool.Singleton.ReturnNetworkObject(entityNet);
    }
}