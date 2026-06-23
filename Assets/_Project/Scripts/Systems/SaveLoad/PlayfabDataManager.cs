using System;
using System.Collections.Generic;
using FeatureToggles;
using PlayFab;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class PlayfabDataManager : Singleton<PlayfabDataManager>
{
    private const float SessionHeartbeatIntervalSeconds = 10f;

    public ActionNavigationSpecificScreen navigationToCharacterSelectionScreen;

    public event Action<GameData> OnLoadGameFormPlayfab;
    public event Action<List<ItemData>> OnLoadCharacterFormPlayfab;
    public event Action<List<ItemData>> OnCharacterChanged;
    public event Action<AuthResult> LoginSuccess;
    public event Action<AuthError> LoginError;

    [SerializeField] private GameData gameData = new GameData();

    private readonly List<ILoadRemote<GameData>> loadRemotes = new();
    private readonly List<ISaveRemote<GameData>> saveRemotes = new();

    private AuthFacade authFacade;
    private PlayFabDataClientService service;
    private PlayFabRealtimeSessionService realtimeSessionService;
    private ItemCharacterService characterService;
    private PlayFabClientInstanceAPI clientApi;
    private GameDataCenterManager gameDataCenterManager;

    private bool hasLogined;
    private string sessionId;
    private string currentPlayFabId;
    private bool sessionLockAcquired;

    public bool ready;
    public bool IsAuthenticated => hasLogined && sessionLockAcquired && !string.IsNullOrEmpty(currentPlayFabId);

    public AuthFacade GetAuthManager() => authFacade;
    public PlayFabClientInstanceAPI GetClientAPI() => clientApi;
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
        realtimeSessionService = new PlayFabRealtimeSessionService(clientApi);

        if (Configuration.Instance.startwithHost)
        {
            IAuthService authService = new PlayFabAuthCustomService(clientApi, true);
            authFacade = new AuthFacade(authService);
            authFacade.Login(new LoginData(), onSuccess, onError);
            ready = true;
            return;
        }

        if (Configuration.Instance.IsClientBuild())
        {
            IAuthService authService = new PlayFabAuthService(clientApi);
            authFacade = new AuthFacade(authService);
            ready = true;
        }
    }

    private void OnDataCenterReady(GameDataCenter center)
    {
        if (!IsAuthenticated)
        {
            return;
        }

        LoadCharacterDataChoose();
    }

    public void Login(LoginData loginData)
    {
        authFacade.Login(loginData, onSuccess, onError);
    }

    public void Logout()
    {
        MarkLoggedOutLocally();
        ReleaseRealtimeSessionLock(() =>
        {
            authFacade.Logout(_ =>
            {
                SaveLoadManager.Instance.SaveGame();
                ResetLocalSessionState();
                ScreenManagerHub.Instance.ResetAll();
                if (NetworkManager.Singleton != null)
                {
                    NetworkManager.Singleton.Shutdown();
                }
                FeatureManager.Instance.Reset();
            }, _ =>
            {
                ResetLocalSessionState();
                ScreenManagerHub.Instance.ResetAll();
                if (NetworkManager.Singleton != null)
                {
                    NetworkManager.Singleton.Shutdown();
                }
                FeatureManager.Instance.Reset();
            });
        });
    }

    public void ChangeAccount()
    {
        MarkLoggedOutLocally();
        ReleaseRealtimeSessionLock(() =>
        {
            SaveLoadManager.Instance.SaveGame();
            ResetLocalSessionState();
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.Shutdown();
            }
            ScreenManagerHub.Instance.ResetAll();
            var createAccountScene = ScreenManagerHub.Instance.Get("CreateAccount");
            FeatureManager.Instance.Reset();
            createAccountScene.NavigateTo("Panel (CreateNv)");
        });
    }

    private void onError(AuthError error)
    {
        if (!hasLogined && !sessionLockAcquired)
        {
            ResetLocalSessionState();
        }

        LoginError?.Invoke(error);
    }

    public void onSuccess(AuthResult result)
    {
        gameDataCenterManager.onSuccess(result.clientApi);

        if (!Configuration.Instance.IsClientBuild() && !Configuration.Instance.startwithHost)
        {
            return;
        }

        sessionId = string.IsNullOrEmpty(result.sessionId) ? Guid.NewGuid().ToString() : result.sessionId;
        result.sessionId = sessionId;
        currentPlayFabId = result.userId;

        realtimeSessionService.TryAcquireLock(currentPlayFabId, sessionId, () =>
        {
            sessionLockAcquired = true;
            hasLogined = true;
            StartSessionHeartbeat();
            LoginSuccess?.Invoke(result);

            if (gameDataCenterManager.IsReady())
            {
                LoadCharacterDataChoose();
            }
        }, onError);
    }

    private void LoadCharacterDataChoose()
    {
        if (!IsAuthenticated)
        {
            return;
        }

        loadRemotes.Clear();
        saveRemotes.Clear();

        service = new PlayFabDataClientService(clientApi);
        var playerInventoryService = new PlayerInventoryService(service);
        loadRemotes.Add(playerInventoryService);
        saveRemotes.Add(playerInventoryService);

        characterService = new ItemCharacterService(service);
        var gameBaseCharacterService = new GameBaseCharacterService(service);

        characterService.LoadGame(gameData, () =>
        {
            if (!IsAuthenticated)
            {
                return;
            }

            OnLoadCharacterFormPlayfab?.Invoke(gameData.itemCharacterDatas);
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
        if (!IsAuthenticated)
        {
            return;
        }

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
        if (!IsAuthenticated)
        {
            return;
        }

        int total = loadRemotes.Count;
        if (total == 0)
        {
            callback?.Invoke();
            OnLoadGameFormPlayfab?.Invoke(gameData);
            return;
        }

        int completed = 0;
        foreach (var item in loadRemotes)
        {
            item.LoadGame(gameData, () =>
            {
                if (!IsAuthenticated)
                {
                    return;
                }

                completed++;
                if (completed == total)
                {
                    callback?.Invoke();
                    OnLoadGameFormPlayfab?.Invoke(gameData);
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

    private void StartSessionHeartbeat()
    {
        CancelInvoke(nameof(RefreshRealtimeSessionLock));
        InvokeRepeating(nameof(RefreshRealtimeSessionLock), SessionHeartbeatIntervalSeconds, SessionHeartbeatIntervalSeconds);
    }

    private void RefreshRealtimeSessionLock()
    {
        if (!sessionLockAcquired || string.IsNullOrEmpty(currentPlayFabId) || string.IsNullOrEmpty(sessionId))
        {
            return;
        }

        realtimeSessionService.RefreshLock(currentPlayFabId, sessionId, result =>
        {
            if (result != null && !result.valid && result.shouldLogout)
            {
                ForceLogoutFromRemoteSession();
            }
        }, error =>
        {
            Debug.LogWarning($"Refresh realtime session lock failed: {error.message}");
        });
    }

    private void ReleaseRealtimeSessionLock(Action onReleased = null)
    {
        CancelInvoke(nameof(RefreshRealtimeSessionLock));

        if (!sessionLockAcquired || string.IsNullOrEmpty(sessionId))
        {
            onReleased?.Invoke();
            return;
        }

        realtimeSessionService.ReleaseLock(sessionId, () =>
        {
            sessionLockAcquired = false;
            onReleased?.Invoke();
        }, error =>
        {
            Debug.LogWarning($"Release realtime session lock failed: {error.message}");
            sessionLockAcquired = false;
            onReleased?.Invoke();
        });
    }

    protected override void OnApplicationQuit()
    {
        MarkLoggedOutLocally();
        ReleaseRealtimeSessionLock();
        base.OnApplicationQuit();
    }

    protected override void OnDestroy()
    {
        MarkLoggedOutLocally();
        ReleaseRealtimeSessionLock();
        base.OnDestroy();
    }

    private void MarkLoggedOutLocally()
    {
        hasLogined = false;
        CancelInvoke(nameof(RefreshRealtimeSessionLock));
    }

    private void ResetLocalSessionState()
    {
        hasLogined = false;
        sessionLockAcquired = false;
        sessionId = string.Empty;
        currentPlayFabId = string.Empty;
        loadRemotes.Clear();
        saveRemotes.Clear();
        gameData.Clear();
    }

    private void ForceLogoutFromRemoteSession()
    {
        MarkLoggedOutLocally();

        authFacade.Logout(_ => { PostRemoteKick(); }, _ => { PostRemoteKick(); });
    }

    private void PostRemoteKick()
    {
        ResetLocalSessionState();
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
        }

        ScreenManagerHub.Instance.ResetAll();
        FeatureManager.Instance.Reset();
        LoginError?.Invoke(new AuthError("SESSION_REVOKED", "Tai khoan nay vua dang nhap o thiet bi khac."));
    }
}
