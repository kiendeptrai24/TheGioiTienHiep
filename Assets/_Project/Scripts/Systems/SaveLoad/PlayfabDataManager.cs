
using System;
using System.Collections.Generic;
using FeatureToggles;
using PlayFab;
using PlayFab.ClientModels;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class PlayfabDataManager : Singleton<PlayfabDataManager>
{
    public ActionNavigationSpecificScreen navigationToCharacterSelectionScreen;
    #region Callback
    public event Action<GameData> OnLoadGameFormPlayfab;
    public event Action<List<ItemData>> OnLoadCharacterFormPlayfab;
    public event Action<List<ItemData>> OnCharacterChanged;
    public event Action<AuthResult> LoginSuccess;
    public event Action<AuthError> LoginError;
    #endregion

    [SerializeField] private GameData gameData = new GameData();
    private List<ILoadRemote<GameData>> loadRemotes = new();
    private List<ISaveRemote<GameData>> saveRemotes = new();

    private AuthFacade authFacade;
    private PlayFabDataClientService service;
    private ItemCharacterService characterService;
    private PlayFabClientInstanceAPI clientApi;
    public AuthFacade GetAuthManager() => authFacade;
    public PlayFabClientInstanceAPI GetClientAPI() => clientApi;
    private GameDataCenterManager gameDataCenterManager;
    private bool hasLogined = false;
    public bool ready = false;
    private string sessionId;
    private bool netcodeCallbacksRegistered = false;
    public List<ItemData> GetCharactersData() => gameData.itemCharacterDatas;

    protected override void Awake()
    {
        base.Awake();
        gameDataCenterManager = GameDataCenterManager.Instance;
        gameDataCenterManager.OnLoadGameDataCenterSuccessed += OnDataCenterReady;
        navigationToCharacterSelectionScreen = GetComponent<ActionNavigationSpecificScreen>();
    }
    protected override void Start()
    {
        base.Start();
        ConfigAuthen();
        if (Configuration.Instance.IsClientBuild() || Configuration.Instance.startwithHost)
        {
            authFacade.AutoLogin(onSuccess, onError);
        }
    }
    private void ConfigAuthen()
    {
        clientApi = new PlayFabClientInstanceAPI(PlayFabSettings.staticSettings);
        if (Configuration.Instance.startwithHost)
        {
            IAuthService authService = new PlayFabAuthCustomService(clientApi, true);
            authFacade = new AuthFacade(authService);
            authFacade.Login(new LoginData(), onSuccess, onError);
            ready = true;
        }
        else if (Configuration.Instance.IsClientBuild())
        {
            IAuthService authService = new PlayFabAuthService(clientApi);
            authFacade = new AuthFacade(authService);
            ready = true;
        }
    }
    private void OnDataCenterReady(GameDataCenter center)
    {
        if (hasLogined == false) return;
        LoadCharacterDataChoose();
    }

    public void Login(LoginData loginData)
    {
        authFacade.Login(loginData, onSuccess, onError);
    }
    public void Logout()
    {
        authFacade.Logout((result) =>
        {
            SaveLoadManager.Instance.SaveGame();
            ScreenManagerHub.Instance.ResetAll();
            NetworkManager.Singleton.Shutdown();
            FeatureManager.Instance.Reset();
        }, default);
    }
    public void ChangeAccount()
    {
        SaveLoadManager.Instance.SaveGame();
        NetworkManager.Singleton.Shutdown();
        ScreenManagerHub.Instance.ResetAll();
        var createAccountScene = ScreenManagerHub.Instance.Get("CreateAccount");
        FeatureManager.Instance.Reset();
        createAccountScene.NavigateTo("Panel (CreateNv)");
    }
    private void onError(AuthError error)
    {
        LoginError?.Invoke(error);
    }

    public void onSuccess(AuthResult result)
    {
        gameDataCenterManager.onSuccess(result.clientApi);

        if (Configuration.Instance.IsClientBuild())
        {
            sessionId = result.sessionId;
            //CreateSession();
            LoginSuccess?.Invoke(result);
            hasLogined = true;
            if (gameDataCenterManager.IsReady() == false) return;
            LoadCharacterDataChoose();
        }
    }

    private void LoadCharacterDataChoose()
    {
        loadRemotes.Clear();
        service = new PlayFabDataClientService(clientApi);
        loadRemotes.Add(new PlayerInventoryService(service));
        saveRemotes.Add(new PlayerInventoryService(service));

        characterService = new ItemCharacterService(service);
        var gameBaseCharacterService = new GameBaseCharacterService(service);

        characterService.LoadGame(gameData, () =>
        {
            OnLoadCharacterFormPlayfab?.Invoke(this.gameData.itemCharacterDatas);
            saveRemotes.Add(characterService);
        });
        loadRemotes.Add(gameBaseCharacterService);
    }

    public void AddCharacter(ItemData itemCharacter)
    {
        var heroData = itemCharacter as HeroData;

        if (heroData == null)
        {
            Debug.LogError("AddCharacter failed: itemCharacter is not HeroData");
            return;
        }
        gameData.Clear();
        heroData.isCharacter = true;
        gameData.createdAt = TimeUtils.GetCurrentTimeString();
        gameData.characterName = itemCharacter.itemName;
        gameData.characterId = heroData.characterId;
        gameData.itemDatas.Add(itemCharacter);
        gameData.itemCharacterDatas.Add(itemCharacter);
        OnCharacterChanged?.Invoke(gameData.itemCharacterDatas);

        gameData.potentialPoint = heroData.realmData.rewardPotentialPoint;
        gameData.skillPoint = heroData.realmData.rewardSkillPoint;
        var realmData = GameDataCenterManager.Instance.GetItemById(heroData.realmId);
        if (realmData != null)
        {
            gameData.currentHealth = (int)realmData.health;
            gameData.currentMana = (int)realmData.mana;
            gameData.currentSpirit = (int)realmData.spirit;
        }
        characterService.SaveGame(gameData);
        SaveGameData();
    }
    public void OnCharacterLoaded(string characterId)
    {
        gameData.Clear();
        gameData.characterId = characterId;
        Debug.Log("OnCharacterLoaded: " + characterId);
        SceneLoadManager.Instance.LoadSceneLoading();
        LoadGameData(() =>
        {
            bool canUnloadLoadingScene = false;

            if (Configuration.Instance.startwithHost)
            {
                canUnloadLoadingScene = NetworkManager.Singleton.StartHost();
                if (!canUnloadLoadingScene)
                {
                    Debug.LogError("StartHost failed, keep LoadingScene open.");
                }
            }
            else
            {
                if (Configuration.Instance.IsClientRemoteBuild())
                {
                    var utp = NetworkManager.Singleton.NetworkConfig.NetworkTransport as UnityTransport;
                    if (utp != null)
                    {
                        var config = Configuration.Instance;
                        if (config != null)
                        {
                            var ipAddress = config.ipAddress;
                            var port = config.port;
                            if (string.IsNullOrEmpty(ipAddress) || port == 0)
                            {
                                Debug.Log("Invalid IP address or port in configuration. Please check the settings.");
                                return;
                            }
                            utp.SetConnectionData(config.ipAddress, config.port);
                        }
                    }
                }

                canUnloadLoadingScene = NetworkManager.Singleton.StartClient();
                if (!canUnloadLoadingScene)
                {
                    Debug.LogError("StartClient failed, keep LoadingScene open.");
                }
            }

            if (canUnloadLoadingScene)
            {
                SceneLoadManager.Instance.UnLoadScene("LoadingScene");
            }
        });
    }
    private void LoadGameData(Action callback)
    {
        int total = loadRemotes.Count;
        int completed = 0;

        foreach (var item in loadRemotes)
        {
            item.LoadGame(gameData, () =>
            {
                completed++;
                if (completed == total)
                {
                    callback?.Invoke();
                    OnLoadGameFormPlayfab?.Invoke(this.gameData);
                }
            });
        }
    }
    public void SaveGameData()
    {
        foreach (var save in saveRemotes)
        {
            save.SaveGame(gameData);
        }
    }
}