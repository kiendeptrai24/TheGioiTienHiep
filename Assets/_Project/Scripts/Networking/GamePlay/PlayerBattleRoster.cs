using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.Netcode;
using UnityEngine;
[Serializable]
public class ChampionSetUp
{
    public string championId;
    public Vector2Int championIndex;
}
public class PlayerBattleRoster : TGTHNetworkBehaviour
{
    public bool player = false;
    [SerializeField] private List<ChampionSetUp> championSetUps = new();
    [Header("Hero prefabs of this player (must be NetworkObject prefabs + registered in NetworkManager)")]
    public List<ItemData> itemDatas = new();
    // Bạn có thể thêm logic chọn đội hình (chỉ spawn N con đầu tiên)
    public int maxHeroesToSpawn = 5;

    public Action result = default;
    public Action<List<ItemData>> OnChampionPlayerChanged = default;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner && player)
        {
            var itemPrefabDatabase = ItemPrefabDatabase.Instance;
            itemPrefabDatabase.OnPlayerPrefabChanged += OnPlayerPrefabChanged;
            OnPlayerPrefabChanged(itemPrefabDatabase.ListIteDataChampion());
        }
        if (!IsServer) return;
        foreach (var item in championSetUps)
        {
            itemDatas.Clear();
            var itemData = GameDataCenterManager.Instance.GetItemById(item.championId) as HeroData;
            itemData.championIndex = item.championIndex;
            itemDatas.Add(itemData);
        }
    }
    private void OnPlayerPrefabChanged(List<ItemData> list)
    {
        if (!IsSpawned) return;
        itemDatas = list;
        var datasDto = new List<ChampionDataNetDto>();
        foreach (var item in itemDatas)
        {
            datasDto.Add(RuntimeNetDataMapper.ToNetDto(item as HeroData));
        }
        string json = JsonConvert.SerializeObject(datasDto);
        SendToServerOnPlayerPrefabChangedServerRpc(json);
    }
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void SendToServerOnPlayerPrefabChangedServerRpc(string itemDataDTO)
    {
        if (!IsServer) return;
        var datasDto = JsonConvert.DeserializeObject<List<ChampionDataNetDto>>(itemDataDTO);
        var itemDatas = new List<ItemData>();
        foreach (var dto in datasDto)
        {
            itemDatas.Add(RuntimeNetDataMapper.ToHeroData(dto, GameDataCenterManager.Instance));
        }
        this.itemDatas = itemDatas;
        OnChampionPlayerChanged?.Invoke(itemDatas);
    }

    public void GetPlayerTeam(Action result = default)
    {
        this.result = result;
        if (IsServer)
        {
            // Nếu đang chạy trên server luôn thì khỏi RPC
            OnPlayerTeamReceived(OwnerClientId, new List<ItemData>(itemDatas));
            return;
        }
        GetPlayerTeamServerRpc();
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void GetPlayerTeamServerRpc(RpcParams rpcParams = default)
    {
        if (!IsServer) return;

        ulong requesterClientId = rpcParams.Receive.SenderClientId;

        var datasDto = new List<ChampionDataNetDto>();
        foreach (var item in itemDatas)
        {
            datasDto.Add(RuntimeNetDataMapper.ToNetDto(item as HeroData));
        }
        string json = JsonConvert.SerializeObject(datasDto);
        ReturnPlayerTeamClientRpc(requesterClientId, json);
    }

    /// <summary>
    /// Server trả dữ liệu team lại cho đúng client đã request.
    /// </summary>
    [Rpc(SendTo.NotServer)]
    private void ReturnPlayerTeamClientRpc(ulong requesterClientId, string itemDataDTO)
    {
        if (NetworkManager.Singleton.LocalClientId != requesterClientId)
            return;

        var datasDto = JsonConvert.DeserializeObject<List<ChampionDataNetDto>>(itemDataDTO);
        var list = new List<ItemData>();
        foreach (var dto in datasDto)
        {
            list.Add(RuntimeNetDataMapper.ToHeroData(dto, GameDataCenterManager.Instance));
        }
        OnPlayerTeamReceived(requesterClientId, list);
        Debug.Log($"Received team of player {OwnerClientId}, count = {list?.Count ?? 0}");
    }
    public void OnPlayerTeamReceived(ulong requesterClientId, List<ItemData> list)
    {
        itemDatas = list;
        result?.Invoke();
    }
}
