using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerBattleRoster : TGTHNetworkBehaviour
{
    [Header("Hero prefabs of this player (must be NetworkObject prefabs + registered in NetworkManager)")]
    public List<NetworkObject> heroNetPrefabs = new();
    public List<GameObject> chamPrefabs = new();
    // Bạn có thể thêm logic chọn đội hình (chỉ spawn N con đầu tiên)
    public int maxHeroesToSpawn = 5;
    protected override void Awake()
    {
        base.Awake();
        foreach (var cham in chamPrefabs)
        {
            var stats = cham.GetComponent<StatsData>();
            stats.SetupDataPreset();
        }
    }
    protected override void Start()
    {
        if (!IsOwner) return;
        base.Start();
        ItemPrefabDatabase.Instance.OnPlayerPrefabChanged += OnPlayerPrefabChanged;
    }

    private void OnPlayerPrefabChanged(List<GameObject> list)
    {
        chamPrefabs = list;
    }
}
