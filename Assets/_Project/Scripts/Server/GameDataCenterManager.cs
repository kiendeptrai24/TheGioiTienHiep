

using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using Newtonsoft.Json;
using System;

public class GameDataCenterManager : TGTHMonoBehaviour
{
    [SerializeField] private string fileName;
    [SerializeField] private bool encryptData;
    private Dictionary<string, ItemData> allItemsById = new();
    [SerializeField] private List<ItemData> allItems;
    private Dictionary<string, ItemShop> shopItemsById = new();
    [SerializeField] private List<ItemShop> shopItems;

    [SerializeField] private GameDataCenter gameDatas;
    private PlayFabDataServerService service;
    private PlayFabClientInstanceAPI clientApi;
    private Queue<ILoadRemoteServer> saveLoadRemotes = new();
    FileDataHandler<GameDataCenter> fileDataHandler;
    public event Action<GameDataCenter> OnLoadGameDataCenterSuccessed;
    private string serverVersion = "";
    private string localVersion = "flkdsajhfosadhgoidsanfoiaweoif";
    protected override void Awake()
    {
        if (Configuration.Instance.buildType == BuildType.LOCAL_CLIENT ||
            Configuration.Instance.buildType == BuildType.REMOTE_CLIENT)
            return;
        fileDataHandler = new FileDataHandler<GameDataCenter>(Application.persistentDataPath, fileName, encryptData);
        Debug.Log(Application.persistentDataPath);
        clientApi = new PlayFabClientInstanceAPI(PlayFabSettings.staticSettings);
        var request = new LoginWithCustomIDRequest { CustomId = "Server", CreateAccount = true };
        clientApi.LoginWithCustomID(request, onSuccess, onError);
    }

    private void onError(PlayFabError error)
    {
        Debug.Log(error.Error);
    }

    private void onSuccess(LoginResult result)
    {

        clientApi.GetTitleData(new GetTitleDataRequest
        {
            Keys = new List<string> { "game_data_version" }
        },
        result =>
        {
            serverVersion = result.Data["game_data_version"];
            gameDatas = fileDataHandler.Load();
            if (gameDatas != null)
                localVersion = gameDatas.version;

            if (serverVersion != localVersion)
            {
                gameDatas = new();
                RequestDataCloud();
            }
            else
            {
                LoadLocalData();
            }

        },
        error => Debug.LogError(error.GenerateErrorReport()));
    }

    private void LoadLocalData()
    {
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

        ConfigDataCenter();
    }

    private void RequestDataCloud()
    {
        service = new PlayFabDataServerService(clientApi);
        saveLoadRemotes.Enqueue(new RealmService(service));
        saveLoadRemotes.Enqueue(new EssenceAndRaceService(service));
        saveLoadRemotes.Enqueue(new InventoryService(service));
        saveLoadRemotes.Enqueue(new ChampionService(service));
        saveLoadRemotes.Enqueue(new ShopService(service));
        LoadGameData();
    }

    private void LoadGameData()
    {
        LoadNextService();
    }

    private void LoadNextService()
    {
        if (saveLoadRemotes.Count <= 0)
        {
            ConfigDataCenter();
            gameDatas.version = serverVersion;
            fileDataHandler.Save(gameDatas);
            return;
        }

        var loadService = saveLoadRemotes.Dequeue();

        loadService.LoadGame(gameDatas, () =>
        {
            LoadNextService();
        });
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
        OnLoadGameDataCenterSuccessed?.Invoke(gameDatas);
    }
    [ContextMenu("Clear Data Cache")]
    public void ClearDataLocalCache()
    {
        FileDataHandler<GameDataCenter> fileDataHandler = new FileDataHandler<GameDataCenter>(Application.persistentDataPath, fileName, encryptData);
        fileDataHandler.Delete();
    }
}