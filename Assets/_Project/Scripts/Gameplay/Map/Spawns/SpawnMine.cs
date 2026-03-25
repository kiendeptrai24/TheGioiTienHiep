using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class SpawnMine : TGTHNetworkBehaviour
{
    [SerializeField] private SpawnSettings settings;
    [SerializeField] private GameObject prefab;
    [SerializeField] private SpawnManager spawnManager;
    protected override void Awake()
    {
        spawnManager = GetComponent<SpawnManager>();
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

        ISpawnArea area = new RectSpawnArea(new Vector3(100, 0, 100), new Vector2(200, 200));
        ISpawnPattern pattern = new RandomSpawnPattern();
        spawnManager.SpawnNetwork(prefab, area, pattern, settings);
    }

}