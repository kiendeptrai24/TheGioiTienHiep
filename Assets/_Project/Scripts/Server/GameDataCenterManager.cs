

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
    PlayFabDataServerService service;
    protected override void Awake()
    {
        fileDataHandler = new FileDataHandler<GameDataCenter>(Application.persistentDataPath, fileName, encryptData);
        service = new PlayFabDataServerService();
        LoadData();
    }
    public ItemData GetItemById(string id)
    {
        if (allItemsById.ContainsKey(id))
            return allItemsById[id].Clone();
        return null;
    }
    public ItemData GetShopItemById(string id)
    {
        if (shopItemsById.ContainsKey(id))
            return shopItemsById[id].Clone();
        return null;
    }

    public bool IsReady() => DataCenterReady;
    private void LoadData()
    {
        gameDatas = fileDataHandler.Load();
        if (gameDatas != null)
        {
            localVersion = gameDatas.version;
        }
        else
        {
            onSuccess();
        }
    }

    public void onSuccess(PlayFabClientInstanceAPI clientApi = null)
    {
        if (clientApi != null)
            this.clientApi = clientApi;

        LoadVersionRemove((sameVersion) =>
        {
            if (sameVersion)
                LoadDataLocal();
            else
                LoadDataRemote(clientApi);
        });
    }
    private void LoadVersionRemove(Action<bool> callback)
    {
        if (Configuration.Instance.IsClientLocalBuild())
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
        else
        {
            PlayFabServerAPI.GetTitleData(new PlayFab.ServerModels.GetTitleDataRequest
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


    }

    private void LoadDataLocal()
    {
        LoadAllData();
        LoadSprite();
        ConfigDataCenter();
        ResolveAllReferences();
        SetupShop();
        ConfigShopDataCenter();
        DataCenterReady = true;
        OnLoadGameDataCenterSuccessed?.Invoke(gameDatas);
        if (Configuration.Instance.IsServerBuild())
        {
            ServerStartUp.Instance.StartServer();
        }
    }

    private void LoadDataRemote(PlayFabClientInstanceAPI clientApi)
    {
        if (Configuration.Instance.IsClientBuild())
        {
            var service = new PlayFabClientGetDataServerService(clientApi);
            var loadDataRemote = new AllGameDataSerice(service);
            gameDatas = new GameDataCenter();
            loadDataRemote.LoadGame(gameDatas, OnLoadDataRemoteSuccessed);
        }
        else
        {

            var loadDataRemote = new AllGameDataSerice(this.service);
            gameDatas = new GameDataCenter();
            loadDataRemote.LoadGame(gameDatas, OnLoadDataRemoteSuccessed);
        }
    }

    private void OnLoadDataRemoteSuccessed()
    {
        try
        {
            ConfigDataCenter();
            LoadSprite();
            ResolveAllReferences();
            SetupShop();
            ConfigShopDataCenter();

            DataCenterReady = true;
            gameDatas.version = serverVersion;
            fileDataHandler.Save(gameDatas);
            OnLoadGameDataCenterSuccessed?.Invoke(gameDatas);
            if (Configuration.Instance.IsServerBuild())
            {
                ServerStartUp.Instance.StartServer();
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"OnLoadDataRemoteSuccessed: Failed to load data remote - {ex.Message}");
        }
    }
    private void LoadSprite()
    {
        try
        {
            foreach (var item in gameDatas.allItems)
            {
                if (item == null) continue;
                item.itemIcon = Resources.Load<Sprite>(item.itemIconPath);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"LoadSprite: Failed to load sprites - {ex.Message}");
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
        foreach (var item in gameDatas.characterDatas)
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
        foreach (var item in gameDatas.pillDatas)
        {
            gameDatas.allItems.Add(item);
        }
    }
    private void ResolveAllReferences()
    {
        try
        {
            foreach (var item in gameDatas.championItems)
            {
                var essenceData = GetItemById(item.essenceId) as EssenceData;
                var raceData = GetItemById(item.raceId) as RaceData;
                var realmData = GetItemById(item.realmId) as RealmData;
                item.realmData = realmData;
                item.essenceData = essenceData;
                item.raceData = raceData;
                foreach (var technique in item.techniqueIds)
                {
                    var techniqueData = GetItemById(technique) as TechniqueData;
                    if (techniqueData != null)
                        item.techniqueDatas.Add(techniqueData);
                }
                foreach (var skill in item.skillIds)
                {
                    var skillData = GetItemById(skill) as SkillData;
                    if (skillData != null)
                        item.skillDatas.Add(skillData);
                }
            }
            foreach (var item in gameDatas.characterDatas)
            {
                item.realmData = GetItemById(item.realmId) as RealmData;
                item.realmType = item.realmData.realmType;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"ResolveAllReferences: Failed to resolve references - {ex.Message}");
        }
    }
    private void SetupShop()
    {
        try
        {
            var shopItem = gameDatas.shopItems.ToList();
            gameDatas.shopItems.Clear();
            for (int i = 0; i < shopItem.Count; i++)
            {
                var item = shopItem[i];
                var itemData = GetItemById(item.instanceId);
                if (itemData != null)
                {
                    itemData.itemPrice = item.itemPrice;
                }
                gameDatas.shopItems.Add(itemData);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"SetupShop: Failed to setup shop - {ex.Message}");
        }
    }

    private void ConfigDataCenter()
    {
        try
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
        catch (Exception ex)
        {
            Debug.LogError($"ConfigDataCenter: Failed to load data - {ex.Message}");
        }
    }

    private void ConfigShopDataCenter()
    {
        try
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
        catch (System.Exception)
        {
            Debug.LogError($"ConfigShopDataCenter: Failed to config shop data center");
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
    public List<ItemData> GetShopDatas() => shopItems;
    public List<HeroData> GetChampionDatas() => gameDatas.championItems;
}