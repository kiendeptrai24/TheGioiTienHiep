using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
[Serializable]
public class ChampionSetUp
{
    public ItemPreset champion;
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

        if (IsOwner)
        {
            var itemPrefabDatabase = ItemPrefabDatabase.Instance;
            itemPrefabDatabase.OnPlayerPrefabChanged += OnPlayerPrefabChanged;
            OnPlayerPrefabChanged(itemPrefabDatabase.ListIteDataChampion());
        }
        if (!IsServer) return;
        foreach (var item in championSetUps)
        {
            var itemData = item.champion.GetItemData() as HeroData;
            itemData.championIndex = item.championIndex;
            itemDatas.Add(itemData);
        }
    }
    private void OnPlayerPrefabChanged(List<ItemData> list)
    {
        itemDatas = list;
        string json = ItemJsonConverter.ToJson(list);
        SendToServerOnPlayerPrefabChangedServerRpc(json);
    }
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void SendToServerOnPlayerPrefabChangedServerRpc(string itemDataDTO)
    {
        if (!IsServer) return;
        var itemDatas = ItemJsonConverter.FromJson(itemDataDTO);
        foreach (var item in itemDatas)
        {
            var itemData = item as HeroData;
            Debug.Log(itemData.championIndex);
        }
        this.itemDatas = itemDatas;
        OnChampionPlayerChanged?.Invoke(itemDatas);
    }
    /// <summary>
    /// Client gọi hàm này trên object roster của người chơi mà mình muốn lấy team.
    /// Ví dụ muốn lấy team đối thủ thì gọi opponentRoster.GetPlayerTeam();
    /// </summary>
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

    /// <summary>
    /// Client request server lấy team của object roster này.
    /// </summary>
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void GetPlayerTeamServerRpc(RpcParams rpcParams = default)
    {
        if (!IsServer) return;

        ulong requesterClientId = rpcParams.Receive.SenderClientId;
        string json = ItemJsonConverter.ToJson(itemDatas);
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

        var list = ItemJsonConverter.FromJson(itemDataDTO);
        OnPlayerTeamReceived(requesterClientId, list);
        Debug.Log($"Received team of player {OwnerClientId}, count = {list?.Count ?? 0}");
    }
    public void OnPlayerTeamReceived(ulong requesterClientId, List<ItemData> list)
    {
        itemDatas = list;
        result?.Invoke();
    }
}
