using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnMine : SingletonNetwork<SpawnMine>, ISpawnable
{
    [SerializeField] private SpawnSettings settings;
    [SerializeField] private int maxObject = 50;
    private ISpawnArea area;
    private ISpawnPattern pattern;
    [SerializeField] private GameObject prefab;
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] public List<NetworkObject> mineNetObjects = new();
    protected override void Awake()
    {
        spawnManager = GetComponent<SpawnManager>();
        if(settings == null) return;
        settings.count = maxObject;
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer) return;
        StartCoroutine(WaitToSpawn());
    }
    private IEnumerator WaitToSpawn()
    {
        yield return null;

        area = new RectSpawnArea(new Vector3(100, 0, 100), new Vector2(200, 200));
        pattern = new RandomSpawnPattern();
        spawnManager.SpawnNetwork(prefab, area, pattern, settings);
    }
    private void CheckObjectEnough()
    {
        if (mineNetObjects.Count < maxObject)
        {
            settings.count = maxObject - mineNetObjects.Count;
            spawnManager.SpawnNetwork(prefab, area, pattern, settings);
        }
    }
    public void RemoveNetObject(NetworkObject entityObject)
    {
        if (!IsServer) return;
        mineNetObjects.Remove(entityObject);
        NetworkObjectPool.Singleton.ReturnNetworkObject(entityObject);
        CheckObjectEnough();
    }

    public void AddNetObject(NetworkObject entityObject)
    {
        if (!IsServer) return;
        mineNetObjects.Add(entityObject);
    }
}