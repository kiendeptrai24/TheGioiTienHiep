using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnService : TGTHNetworkBehaviour
{
    public void SpawnNetwork(List<GameObject> listPrefab, ISpawnArea area, ISpawnPattern pattern, SpawnSettings settings, Transform parent = null)
    {
        if (!IsServer)
        {
            Debug.LogWarning($"[SpawnService] SpawnNetwork called but IsServer=false. Skipping spawn.");
            return;
        }

        if (listPrefab == null || listPrefab.Count == 0)
        {
            Debug.LogError("[SpawnService] listPrefab is null or empty! Assign prefabs in the inspector.");
            return;
        }

        if (NetworkObjectPool.Singleton == null)
        {
            Debug.LogError("[SpawnService] NetworkObjectPool.Singleton is null! Make sure NetworkObjectPool exists in scene.");
            return;
        }

        if (!NetworkObjectPool.Singleton.IsReady)
        {
            Debug.LogWarning($"[SpawnService] NetworkObjectPool not ready yet. Starting coroutine to wait...");
            StartCoroutine(WaitAndSpawn(listPrefab, area, pattern, settings, parent));
            return;
        }

        DoSpawn(listPrefab, area, pattern, settings, parent);
    }

    private IEnumerator WaitAndSpawn(List<GameObject> listPrefab, ISpawnArea area, ISpawnPattern pattern, SpawnSettings settings, Transform parent)
    {
        float waitTime = 0f;
        float maxWait = 10f;

        while (NetworkObjectPool.Singleton != null && !NetworkObjectPool.Singleton.IsReady && waitTime < maxWait)
        {
            yield return new WaitForSeconds(0.1f);
            waitTime += 0.1f;
        }

        if (NetworkObjectPool.Singleton == null || !NetworkObjectPool.Singleton.IsReady)
        {
            Debug.LogError($"[SpawnService] Timed out waiting for NetworkObjectPool to be ready (waited {waitTime}s).");
            yield break;
        }

        Debug.Log($"[SpawnService] Pool is now ready after {waitTime}s. Proceeding with spawn.");
        DoSpawn(listPrefab, area, pattern, settings, parent);
    }

    private void DoSpawn(List<GameObject> listPrefab, ISpawnArea area, ISpawnPattern pattern, SpawnSettings settings, Transform parent)
    {
        List<Vector3> points = pattern.GeneratePoints(area, settings);

        Debug.Log($"[SpawnService] Spawning {points.Count} objects from {listPrefab.Count} prefab types.");

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