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
    [SerializeField] private List<GameObject> listPrefab;
    [SerializeField] private SpawnService spawnManager;
    [SerializeField] public List<NetworkObject> monsterNetObjects = new();
    protected override void Awake()
    {
        base.Awake();
        spawnManager = GetComponent<SpawnService>();
        if (settings == null)
        {
            Debug.LogError("[SpawnMonster] SpawnSettings is null! Assign it in the inspector.");
            return;
        }
        settings.count = maxObject;
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer)
        {
            Debug.Log($"[SpawnMonster] OnNetworkSpawn - not server, skipping spawn.");
            return;
        }

        Debug.Log($"[SpawnMonster] OnNetworkSpawn - Server detected. Starting spawn sequence... (maxObject={maxObject}, listPrefab count={listPrefab?.Count ?? 0})");
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
        if (listPrefab == null || listPrefab.Count == 0)
        {
            Debug.LogError("[SpawnMonster] listPrefab is null or empty! Cannot spawn monsters. Assign prefabs in the inspector.");
            return;
        }

        if (spawnManager == null)
        {
            Debug.LogError("[SpawnMonster] spawnManager (SpawnService) is null! Make sure it's on the same GameObject.");
            return;
        }

        area = new RectSpawnArea(new Vector3(100, 0, 100), new Vector2(200, 200));
        pattern = new RandomSpawnPattern();

        Debug.Log($"[SpawnMonster] Calling SpawnNetwork with {listPrefab.Count} prefab types, count={settings?.count}");
        spawnManager.SpawnNetwork(listPrefab, area, pattern, settings);
    }
    private void SpawnOne()
    {
        if (!IsServer) return;
        if (monsterNetObjects.Count >= maxObject) return;

        settings.count = 1;
        spawnManager.SpawnNetwork(listPrefab, area, pattern, settings);
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