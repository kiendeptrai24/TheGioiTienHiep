using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnMine : SingletonNetwork<SpawnMine>, INetObjectRegistry
{
    [SerializeField] private SpawnSettings settings;
    [SerializeField] private int maxObject = 50;
    private ISpawnArea area;
    private ISpawnPattern pattern;
    [SerializeField] private List<GameObject> listPrefab;
    [SerializeField] private SpawnService spawnManager;
    [SerializeField] public List<NetworkObject> mineNetObjects = new();


    protected override void Awake()
    {
        spawnManager = GetComponent<SpawnService>();
        if (settings == null) return;
        settings.count = maxObject;
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer) return;
        WaitToSpawn();
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        //RemoveAll();
    }
    private void WaitToSpawn()
    {
        area = new RectSpawnArea(new Vector3(100, 0, 100), new Vector2(200, 200));
        pattern = new RandomSpawnPattern();

        spawnManager.SpawnNetwork(listPrefab, area, pattern, settings);
    }
    
    private void RemoveAll()
    {
        if (!IsServer || IsSpawned == false) return;
        foreach (var netobj in mineNetObjects)
        {
            NetworkObjectPool.Singleton.ReturnNetworkObject(netobj);
        }
        mineNetObjects.Clear();
    }
    private void SpawnOne()
    {
        if (!IsServer) return;
        if (mineNetObjects.Count >= maxObject) return;

        settings.count = 1;
        spawnManager.SpawnNetwork(listPrefab, area, pattern, settings);
    }
    public void RemoveNetObject(NetworkObject entityObject)
    {
        if (!IsServer) return;
        if (entityObject == null) return;
        if (!mineNetObjects.Contains(entityObject)) return;

        mineNetObjects.Remove(entityObject);
        NetworkObjectPool.Singleton.ReturnNetworkObject(entityObject);

        SpawnRespawnTimerManager.Instance.AddRespawnTask(60, () =>
        {
            SpawnOne();
        });
    }

    public void AddNetObject(NetworkObject entityObject)
    {
        if (!IsServer) return;
        if (entityObject == null) return;
        if (mineNetObjects.Contains(entityObject)) return;

        mineNetObjects.Add(entityObject);
    }
}