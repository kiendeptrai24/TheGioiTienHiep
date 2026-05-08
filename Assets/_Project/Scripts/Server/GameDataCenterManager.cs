

using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using System;
using System.Linq;

public class GameDataCenterManager : Singleton<GameDataCenterManager>
{
    #region File Handle

    [SerializeField] private string fileName;
    [SerializeField] private bool encryptData;
    private FileDataHandler<GameDataCenter> fileDataHandler;
    #endregion
    #region Data
    private Dictionary<string, ItemData> allItemsById = new();
    [SerializeField] private List<ItemData> allItems;
    private Dictionary<string, ItemData> shopItemsById = new();
    [SerializeField] private List<ItemData> shopItems;

    [SerializeField] private GameDataCenter gameDatas;

    #endregion
    public event Action<GameDataCenter> OnLoadGameDataCenterSuccessed;
    private bool DataCenterReady = false;

    private PlayFabClientInstanceAPI clientApi;
    private string serverVersion = "";
    private string localVersion = "";
    protected override void Awake()
    {
        fileDataHandler = new FileDataHandler<GameDataCenter>(Application.persistentDataPath, fileName, encryptData);
        if (Configuration.Instance.buildType == BuildType.LOCAL_CLIENT ||
            Configuration.Instance.buildType == BuildType.REMOTE_CLIENT)
            return;
        LoadDataLocalCache();
    }
    public ItemData GetItemById(string id)
    {
        if (allItemsById.ContainsKey(id))
            return allItemsById[id];
        return null;
    }
    public bool IsReady() => DataCenterReady;
    private void LoadDataLocalCache()
    {
        gameDatas = fileDataHandler.Load();
        if (gameDatas != null)
            localVersion = gameDatas.version;
    }

    public void onSuccess(PlayFabClientInstanceAPI client)
    {
        if (client == null) return;
        clientApi = client;

        LoadVersionRemove((sameVersion) =>
        {
            if (sameVersion)
                LoadDataLocal();
            else
                LoadDataRemote();
        });
    }
    private void LoadVersionRemove(Action<bool> callback)
    {
        clientApi.GetTitleData(new GetTitleDataRequest
        {
            Keys = new List<string> { "game_data_version" }
        },
        result =>
        {
            serverVersion = result.Data["game_data_version"];
            if (string.IsNullOrEmpty(serverVersion))
            {
                callback?.Invoke(false);
                return;
            }

            callback?.Invoke(serverVersion == localVersion);
        },
        error => Debug.LogError(error.GenerateErrorReport()));

    }

    private void LoadDataLocal()
    {
        LoadAllData();
        ConfigDataCenter();
        ResolveAllReferences();
        SetupShop();
        ConfigShopDataCenter();
        DataCenterReady = true;
        OnLoadGameDataCenterSuccessed?.Invoke(gameDatas);
    }

    private void LoadDataRemote()
    {
        var service = new PlayFabDataServerService(clientApi);
        var loadDataRemote = new AllGameDataSerice(service);
        gameDatas = new GameDataCenter();
        loadDataRemote.LoadGame(gameDatas, OnLoadDataRemoteSuccessed);
    }

    private void OnLoadDataRemoteSuccessed()
    {
        try
        {
            ConfigDataCenter();
            ResolveAllReferences();
            SetupShop();
            ConfigShopDataCenter();

            DataCenterReady = true;
            gameDatas.version = serverVersion;
            fileDataHandler.Save(gameDatas);
            OnLoadGameDataCenterSuccessed?.Invoke(gameDatas);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"OnLoadDataRemoteSuccessed: Failed to load data remote - {ex.Message}");
        }
    }
    private void LoadAllData()
    {
        gameDatas.allItems.Clear();
        foreach (var item in gameDatas.equipmentItems)
        {
            gameDatas.allItems.Add(item);
        }
        foreach (var item in gameDatas.skillDatas)
        {
            gameDatas.allItems.Add(item);
        }
        foreach (var item in gameDatas.techniqueDatas)
        {
            gameDatas.allItems.Add(item);
        }
        foreach (var item in gameDatas.essenceItems)
        {
            gameDatas.allItems.Add(item);
        }
        foreach (var item in gameDatas.raceItems)
        {
            gameDatas.allItems.Add(item);
        }
        foreach (var item in gameDatas.championItems)
        {
            gameDatas.allItems.Add(item);
        }
        foreach (var item in gameDatas.realmDatas)
        {
            gameDatas.allItems.Add(item);
        }
        foreach (var item in gameDatas.spiritStoneMineItems)
        {
            gameDatas.allItems.Add(item);
        }
        foreach (var item in gameDatas.demonBeastItems)
        {
            gameDatas.allItems.Add(item);
        }
    }
    private void ResolveAllReferences()
    {
        foreach (var item in gameDatas.championItems)
        {
            var essenceData = GetItemById(item.essenceId).Clone() as EssenceData;
            var raceData = GetItemById(item.raceId).Clone() as RaceData;
            var realmData = GetItemById(item.realmId).Clone() as RealmData;
            item.realmData = realmData;
            item.essenceData = essenceData;
            item.raceData = raceData;
            foreach (var technique in item.techniqueIds)
            {
                var techniqueData = GetItemById(technique).Clone() as TechniqueData;
                if (techniqueData != null)
                    item.techniqueDatas.Add(techniqueData);
            }
            foreach (var skill in item.skillIds)
            {
                var skillData = GetItemById(skill).Clone() as SkillData;
                if (skillData != null)
                    item.skillDatas.Add(skillData);
            }
        }
    }

    private void SetupShop()
    {
        var shopItem = gameDatas.shopItems.ToList();
        gameDatas.shopItems.Clear();
        for (int i = 0; i < shopItem.Count; i++)
        {
            var item = shopItem[i];
            var itemData = GetItemById(item.instanceId).Clone();
            if (itemData != null)
            {
                itemData.itemPrice = item.itemPrice;
            }
            gameDatas.shopItems.Add(itemData);
        }
    }

    private void ConfigDataCenter()
    {
        allItems.Clear();
        allItemsById.Clear();

        foreach (var item in gameDatas.allItems)
        {
            if (item == null || string.IsNullOrEmpty(item.instanceId))
                continue;

            if (allItemsById.ContainsKey(item.instanceId))
            {
                Debug.LogWarning($"Duplicate item id: {item.instanceId}");
                continue;
            }

            allItemsById.Add(item.instanceId, item);
            allItems.Add(item);
        }
    }

    private void ConfigShopDataCenter()
    {
        shopItems.Clear();
        shopItemsById.Clear();
        foreach (var item in gameDatas.shopItems)
        {
            if (shopItemsById.ContainsKey(item.instanceId))
            {
                Debug.LogWarning($"Duplicate item id: {item.instanceId}");
                continue;
            }
            shopItemsById.Add(item.instanceId, item);
            shopItems.Add(item);
        }
    }

    [ContextMenu("Clear Data Cache")]
    private void ClearDataLocalCache()
    {
        FileDataHandler<GameDataCenter> fileDataHandler = new FileDataHandler<GameDataCenter>(Application.persistentDataPath, fileName, encryptData);
        fileDataHandler.Delete();
    }
    public List<ItemData> GetAllCharacters() => gameDatas.characterDatas.ToList<ItemData>();
    public GameDataCenter GetDataCenter() => gameDatas;
    public List<ItemData> GetShopItems() => shopItems;
}