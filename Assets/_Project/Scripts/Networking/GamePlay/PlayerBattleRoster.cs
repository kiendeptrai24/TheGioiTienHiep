using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
[System.Serializable]
public class ChampionSetUp
{
    public ItemPreset champion;
    public Vector2Int championIndex;
}
public class PlayerBattleRoster : TGTHNetworkBehaviour
{
    [SerializeField] private List<ChampionSetUp> championSetUps = new();
    [Header("Hero prefabs of this player (must be NetworkObject prefabs + registered in NetworkManager)")]
    public List<ItemData> itemDatas = new();
    // Bạn có thể thêm logic chọn đội hình (chỉ spawn N con đầu tiên)
    public int maxHeroesToSpawn = 5;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer) return;
        foreach (var item in championSetUps)
        {
            var itemData = item.champion.GetItemData() as HeroData;
            itemData.championIndex = item.championIndex;
            itemDatas.Add(itemData);
        }
    }
    protected override void Start()
    {
        if (!IsOwner) return;
        base.Start();
        ItemPrefabDatabase.Instance.OnPlayerPrefabChanged += OnPlayerPrefabChanged;
    }
    private void OnPlayerPrefabChanged(List<ItemData> list)
    {
        itemDatas = list;
    }
}
