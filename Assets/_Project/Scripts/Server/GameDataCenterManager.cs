

using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using System;

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
    private Dictionary<string, ItemShop> shopItemsById = new();
    [SerializeField] private List<ItemShop> shopItems;

    [SerializeField] private GameDataCenter gameDatas;

    #endregion
    public event Action<GameDataCenter> OnLoadGameDataCenterSuccessed;
    public bool DataCenterReady = false;

    private PlayFabClientInstanceAPI clientApi;
    private string serverVersion = "";
    private string localVersion = "";
    protected override void Awake()
    {
        if (Configuration.Instance.buildType == BuildType.LOCAL_CLIENT ||
            Configuration.Instance.buildType == BuildType.REMOTE_CLIENT)
            return;
        LoadDataLocalCache();
    }

    private void LoadDataLocalCache()
    {
        fileDataHandler = new FileDataHandler<GameDataCenter>(Application.persistentDataPath, fileName, encryptData);
        gameDatas = fileDataHandler.Load();
        if (gameDatas == null)
            localVersion = null;
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
    public void LoadVersionRemove(Action<bool> callback)
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
        SetupDataWithId();
        DataCenterReady = true;
        OnLoadGameDataCenterSuccessed?.Invoke(gameDatas);
    }

    private void LoadDataRemote()
    {
        var service = new PlayFabDataServerService(clientApi);
        var loadDataRemote = new AllGameDataSerice(service);
        loadDataRemote.LoadGame(gameDatas, () =>
        {
            gameDatas.version = serverVersion;
            ConfigDataCenter();
            SetupDataWithId();
            fileDataHandler.Save(gameDatas);
            DataCenterReady = true;
            OnLoadGameDataCenterSuccessed?.Invoke(gameDatas);
        });
    }


    private void LoadAllData()
    {
        gameDatas.allItems.Clear();
        foreach (var item in gameDatas.equipmentItems)
        {
            gameDatas.allItems.Add(item);
        }
        foreach (var item in gameDatas.skillItems)
        {
            gameDatas.allItems.Add(item);
        }
        foreach (var item in gameDatas.techniqueDatasItems)
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
        foreach (var item in gameDatas.realmItems)
        {
            gameDatas.allItems.Add(item);
        }
    }

    private void SetupDataWithId()
    {
        foreach (var item in gameDatas.championItems)
        {
            if (allItemsById.ContainsKey(item.essenceId))
            {
                var essenceData = allItemsById[item.essenceId] as EssenceData;
                if (essenceData != null)
                {
                    item.essenceData = essenceData;
                    item.essenceType = essenceData.essenceType;
                }
            }
            if (allItemsById.ContainsKey(item.raceId))
            {
                var raceData = allItemsById[item.raceId] as RaceData;
                if (item.raceData != null)
                {
                    item.raceData = raceData;
                    item.raceType = raceData.raceType;
                }
            }
            if (allItemsById.ContainsKey(item.realmId))
            {
                var realmData = allItemsById[item.realmId] as RealmData;
                if (realmData != null)
                {
                    item.realmData = realmData;
                    item.realmType = realmData.realmType;
                }
            }
            foreach (var technique in item.techniqueIds)
            {
                if (allItemsById.ContainsKey(technique))
                    item.techniqueDatas.Add(allItemsById[technique] as TechniqueData);
            }
            foreach (var skill in item.skillIds)
            {
                if (allItemsById.ContainsKey(skill))
                    item.skillDatas.Add(allItemsById[skill] as SkillData);
            }
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
    public void ClearDataLocalCache()
    {
        FileDataHandler<GameDataCenter> fileDataHandler = new FileDataHandler<GameDataCenter>(Application.persistentDataPath, fileName, encryptData);
        fileDataHandler.Delete();
    }
}