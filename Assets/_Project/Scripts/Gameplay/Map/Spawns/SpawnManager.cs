using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnManager : TGTHNetworkBehaviour
{
    public void SpawnNetwork(GameObject prefab, ISpawnArea area, ISpawnPattern pattern, SpawnSettings settings, Transform parent = null)
    {
        if (!IsServer) return;
        List<Vector3> points = pattern.GeneratePoints(area, settings);

        foreach (var point in points)
        {
            NetworkObject entityNet = NetworkObjectPool.Singleton.GetNetworkObject(prefab, point, Quaternion.identity);
        }
    }
    public void ReturnToPool(NetworkObject entityNet)
    {
        NetworkObjectPool.Singleton.ReturnNetworkObject(entityNet);
    
    }
}