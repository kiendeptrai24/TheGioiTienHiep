using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnManager : TGTHNetworkBehaviour
{
    public List<GameObject> Spawn(GameObject prefab, ISpawnArea area, ISpawnPattern pattern, SpawnSettings settings, Transform parent = null)
    {
        List<GameObject> spawned = new();
        List<Vector3> points = pattern.GeneratePoints(area, settings);

        foreach (var point in points)
        {
            GameObject go = Instantiate(prefab, point, Quaternion.identity, parent);
            spawned.Add(go);
        }

        return spawned;
    }
    public void SpawnNetwork(GameObject prefab, ISpawnArea area, ISpawnPattern pattern, SpawnSettings settings, Transform parent = null)
    {
        if (!IsServer) return;
        List<Vector3> points = pattern.GeneratePoints(area, settings);

        foreach (var point in points)
        {
            NetworkObject go = NetworkObjectPool.Singleton.GetNetworkObject(prefab, point, Quaternion.identity);
            var itemMapworld = go.GetComponent<ItemMapWorld>();
            ResourceManager.Instance.AddItemMapWorld(itemMapworld);
        }

    }
}