using System;
using System.Collections.Generic;
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

    public List<ItemData> itemDatas = new();
    public HeroData heroData;
    public int maxHeroesToSpawn = 5;

    public Action result;
    public Action<List<ItemData>> OnChampionPlayerChanged;

    private ItemPrefabDatabase itemPrefabDatabase;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            LoadDefaultTeamOnServer();
        }

        if (IsOwner && player)
        {
            itemPrefabDatabase = ItemPrefabDatabase.Instance;

            if (itemPrefabDatabase != null)
            {
                itemPrefabDatabase.OnPlayerPrefabChanged += OnPlayerPrefabChanged;
                OnPlayerPrefabChanged(itemPrefabDatabase.ListIteDataChampion());
            }
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        if (itemPrefabDatabase != null)
        {
            itemPrefabDatabase.OnPlayerPrefabChanged -= OnPlayerPrefabChanged;
            itemPrefabDatabase = null;
        }
    }

    private void LoadDefaultTeamOnServer()
    {
        itemDatas.Clear();

        int count = Mathf.Min(championSetUps.Count, maxHeroesToSpawn);

        for (int i = 0; i < count; i++)
        {
            var setup = championSetUps[i];

            var hero = GameDataCenterManager.Instance.GetItemById(setup.championId) as HeroData;

            if (hero == null)
                continue;

            hero.championIndex = setup.championIndex;
            itemDatas.Add(hero);
        }
    }

    private void OnPlayerPrefabChanged(List<ItemData> list)
    {
            if (!IsSpawned) return;
        if (!IsOwner) return;
        if (list == null) return;

        itemDatas.Clear();

        int count = Mathf.Min(list.Count, maxHeroesToSpawn);
        var dtoArray = new ChampionDataNetDto[count];

        for (int i = 0; i < count; i++)
        {
            if (list[i] is not HeroData hero)
                continue;

            itemDatas.Add(hero);
            dtoArray[i] = RuntimeNetDataMapper.ToNetDto(hero);
        }

        SendToServerOnPlayerPrefabChangedServerRpc(dtoArray);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    private void SendToServerOnPlayerPrefabChangedServerRpc(ChampionDataNetDto[] datasDto)
    {
        if (!IsServer) return;
        if (datasDto == null) return;

        itemDatas.Clear();

        int count = Mathf.Min(datasDto.Length, maxHeroesToSpawn);

        for (int i = 0; i < count; i++)
        {
            var hero = RuntimeNetDataMapper.ToHeroData(
                datasDto[i],
                GameDataCenterManager.Instance
            );

            if (hero == null)
                continue;
            if (hero.isCharacter == true)
                heroData = hero;

            itemDatas.Add(hero);
        }

        OnChampionPlayerChanged?.Invoke(itemDatas);
    }

    public void GetPlayerTeam(Action callback = null)
    {
        result = callback;

        if (IsServer)
        {
            OnPlayerTeamReceived(OwnerClientId, itemDatas);
            return;
        }

        GetPlayerTeamServerRpc();
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void GetPlayerTeamServerRpc(RpcParams rpcParams = default)
    {
        if (!IsServer) return;

        ulong requesterClientId = rpcParams.Receive.SenderClientId;

        int count = Mathf.Min(itemDatas.Count, maxHeroesToSpawn);
        var dtoArray = new ChampionDataNetDto[count];

        for (int i = 0; i < count; i++)
        {
            if (itemDatas[i] is not HeroData hero)
                continue;

            dtoArray[i] = RuntimeNetDataMapper.ToNetDto(hero);
        }

        ReturnPlayerTeamClientRpc(requesterClientId, dtoArray);
    }

    [Rpc(SendTo.NotServer)]
    private void ReturnPlayerTeamClientRpc(ulong requesterClientId, ChampionDataNetDto[] datasDto)
    {
        if (NetworkManager.Singleton.LocalClientId != requesterClientId)
            return;

        var list = new List<ItemData>();

        if (datasDto != null)
        {
            for (int i = 0; i < datasDto.Length; i++)
            {
                var hero = RuntimeNetDataMapper.ToHeroData(
                    datasDto[i],
                    GameDataCenterManager.Instance
                );

                if (hero != null)
                    list.Add(hero);
            }
        }

        OnPlayerTeamReceived(requesterClientId, list);
    }

    private void OnPlayerTeamReceived(ulong requesterClientId, List<ItemData> list)
    {
        if (IsServer == false)
        {
            itemDatas.Clear();

            if (list != null)
                itemDatas.AddRange(list);
        }
        result?.Invoke();
    }
    public void SetCharacterPersent(VitalType type, float persent)
    {
        if (!IsServer) return;
        foreach (var item in itemDatas)
        {
            if (item is not HeroData hero) continue;
            if (hero.isCharacter == false) continue;

            switch (type)
            {
                case VitalType.Health:
                    hero.healthPersent = persent;
                    break;
                case VitalType.Mana:
                    hero.manaPersent = persent;
                    break;
                case VitalType.Spirit:
                    hero.spiritPersent = persent;
                    break;
            }
        }
    }
}