using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnMonster : SingletonNetwork<SpawnMonster>, INetObjectRegistry
{
    [SerializeField] private SpawnSettings settings;
    [SerializeField] private int maxObject = 50;
    private ISpawnArea area;
    private ISpawnPattern pattern;
    [SerializeField] private GameObject prefab;
    [SerializeField] private SpawnService spawnManager;
    [SerializeField] public List<NetworkObject> monsterNetObjects = new();
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
    private void RemoveAll()
    {
        if (!IsServer || IsSpawned == false) return;
        foreach (var netobj in monsterNetObjects)
        {
            NetworkObjectPool.Singleton.ReturnNetworkObject(netobj);
        }
        monsterNetObjects.Clear();
    }

    private void WaitToSpawn()
    {
        area = new RectSpawnArea(new Vector3(100, 0, 100), new Vector2(200, 200));
        pattern = new RandomSpawnPattern();

        spawnManager.SpawnNetwork(prefab, area, pattern, settings);
    }
    private void SpawnOne()
    {
        if (!IsServer) return;
        if (monsterNetObjects.Count >= maxObject) return;

        settings.count = 1;
        spawnManager.SpawnNetwork(prefab, area, pattern, settings);
    }
    public void RemoveNetObject(NetworkObject entityObject)
    {
        if (!IsServer) return;
        if (entityObject == null) return;
        if (!monsterNetObjects.Contains(entityObject)) return;

        monsterNetObjects.Remove(entityObject);
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
        if (monsterNetObjects.Contains(entityObject)) return;

        monsterNetObjects.Add(entityObject);
    }
}