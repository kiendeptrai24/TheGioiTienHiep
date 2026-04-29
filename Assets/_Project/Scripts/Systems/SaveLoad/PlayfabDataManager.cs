
using System;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayfabDataManager : Singleton<PlayfabDataManager>
{
    public event Action<GameData> OnLoadGameFormPlayfab;
    public event Action<List<ItemData>> OnLoadCharacterFormPlayfab;
    public event Action<List<ItemData>> OnCharacterChanged;
    public event Action<List<ItemData>> OnGameBaseCharacterReady;
    public event Action<AuthResult> LoginSuccess;
    public event Action<AuthError> LoginError;
    [SerializeField] private GameData gameData = new GameData();
    private List<ISaveLoadRemote> saveLoadRemotes = new List<ISaveLoadRemote>();
    private AuthManager authManager;
    private PlayFabDataClientService service;
    public List<ItemData> GetCharactersData() => gameData.itemCharacterDatas;
    private ISaveLoadRemote characterService;
    private PlayFabClientInstanceAPI clientApi;
    public AuthManager GetAuthManager() => authManager;
    public PlayFabClientInstanceAPI GetClientAPI() => clientApi;
    public ActionNavigationSpecificScreen navigationToCharacterSelectionScreen;
    public bool ready = false;
    public GameData GetGameData() => gameData;
    private GameDataCenterManager gameDataCenterManager;
    private bool hasLogined = false;
    private string sessionId;

    protected override void Awake()
    {
        base.Awake();
        gameDataCenterManager = GameDataCenterManager.Instance;
        gameDataCenterManager.OnLoadGameDataCenterSuccessed += OnDataCenterReady;
        navigationToCharacterSelectionScreen = GetComponent<ActionNavigationSpecificScreen>();
        clientApi = new PlayFabClientInstanceAPI(PlayFabSettings.staticSettings);
        if (Configuration.Instance.IsServerBuild())
        {
            IAuthService authService = new PlayFabAuthCustomService(clientApi, true);
            authManager = new AuthManager(authService);
            authManager.Login(new LoginData(), onSuccess, onError);
        }
        if (Configuration.Instance.IsClientLocalBuild())
        {
            IAuthService authService = new PlayFabAuthCustomService(clientApi);
            authManager = new AuthManager(authService);
            ready = true;
        }
        else if (Configuration.Instance.IsClientRemoteBuild())
        {
            IAuthService authService = new PlayFabAuthService(clientApi);
            authManager = new AuthManager(authService);
            LobbyController.Instance.OnLobbySearchLobbiesCompleted += (success, lobby) =>
            {
                if (success)
                {
                    if (LobbyController.Instance.HasLobby()) return;
                    LobbyController.Instance.JoinLobby(clientApi.authenticationContext, lobby.ConnectionString);
                    ready = true;
                }
                else
                {
                    if (LobbyController.Instance.HasLobby()) return;
                    var playfabConnectMutiplayer = new PlayfabConnectMutiplayer(clientApi.authenticationContext);
                    playfabConnectMutiplayer.RequestMultiplayerServer(clientApi, Configuration.Instance, result =>
                    {
                        if (result.success)
                        {
                            LobbyController.Instance.CreateLobby(clientApi.authenticationContext, result.ipAddress, result.port);
                            ready = true;
                        }
                        else
                        {
                            Debug.Log("RequestMultiplayerServer failed");
                        }
                    });
                }
            };
        }

    }
    private void StartHeartbeat()
    {
        InvokeRepeating(nameof(SendHeartbeat), 5f, 10f);
    }

    private void SendHeartbeat()
    {
        clientApi.ExecuteCloudScript(new ExecuteCloudScriptRequest
        {
            FunctionName = "Heartbeat",
            FunctionParameter = new
            {
                sessionId = sessionId
            }
        },
        result =>
        {
            var data = result.FunctionResult as IDictionary<string, object>;

            bool valid = Convert.ToBoolean(data["valid"]);

            if (!valid)
            {
                OnKicked();
            }
        },
        error =>
        {
            Debug.LogError(error.GenerateErrorReport());
        });
    }
    public void CreateSession()
    {
        sessionId = Guid.NewGuid().ToString();

        clientApi.ExecuteCloudScript(new ExecuteCloudScriptRequest
        {
            FunctionName = "RequestSession",
            FunctionParameter = new
            {
                sessionId = sessionId
            }
        },
        result =>
        {
            Debug.Log("Session created");
            StartHeartbeat();
        },
        error =>
        {
            Debug.LogError(error.GenerateErrorReport());
        });
    }
    private void OnKicked()
    {
        authManager.Logout(onSuccess, onError);
    }

    private void OnDataCenterReady(GameDataCenter center)
    {
        if (hasLogined == false) return;
        LoadCharacterDataChoose();
    }
    protected override void Start()
    {
        base.Start();
        if (Configuration.Instance.IsServerBuild()) return;
        authManager.AutoLogin(onSuccess, onError);
    }
    public void Login(LoginData loginData)
    {
        authManager.Login(loginData, onSuccess, onError);
    }
    public void Logout()
    {
        authManager.Logout((result) =>
        {
            navigationToCharacterSelectionScreen.OnClick();
        }, default);
    }
    private void onError(AuthError error)
    {
        LoginError?.Invoke(error);
    }

    public void onSuccess(AuthResult result)
    {
        if (Configuration.Instance.IsClientBuild())
        {
            sessionId = result.sessionId;
            CreateSession();
        }

        hasLogined = true;
        FindRemoteServer();
        gameDataCenterManager.onSuccess(result.clientApi);
        LoginSuccess?.Invoke(result);
        if (gameDataCenterManager.DataCenterReady == false) return;
        LoadCharacterDataChoose();
    }

    private void LoadCharacterDataChoose()
    {
        service = new PlayFabDataClientService(clientApi);

        characterService = new ItemCharacterService(service);
        var gameBaseCharacterService = new GameBaseCharacterService(service);

        characterService.LoadGame(gameData, () =>
        {
            OnLoadCharacterFormPlayfab?.Invoke(this.gameData.itemCharacterDatas);
        });
        gameBaseCharacterService.LoadGame(gameData, () =>
        {
            OnGameBaseCharacterReady?.Invoke(this.gameData.gameBaseCharacterDatas);
        });
    }

    private void FindRemoteServer()
    {
        LobbyController.Instance.GetLobbyServer(clientApi.authenticationContext);
    }
    public void AddCharacter(ItemData itemCharacter)
    {
        var heroData = itemCharacter as HeroData;

        gameData.characterName = itemCharacter.itemName;
        gameData.characterId = heroData.characterId;
        gameData.itemDatas.Add(itemCharacter);
        gameData.itemCharacterDatas.Add(itemCharacter);
        OnCharacterChanged?.Invoke(gameData.itemCharacterDatas);

        var playerInventoryService = new PlayerHeroItemInventoryService(service);

        playerInventoryService.SaveGame(gameData);
        characterService.SaveGame(gameData);
    }
    public void OnCharacterLoaded(string characterId)
    {
        saveLoadRemotes.Clear();
        gameData = new GameData();
        gameData.characterId = characterId;
        saveLoadRemotes.Add(new ProfileService(service));
        saveLoadRemotes.Add(new ShopClientService(service));
        saveLoadRemotes.Add(new PlayerUsedItemInventoryService(service));
        saveLoadRemotes.Add(new TeamInventoryService(service));
        saveLoadRemotes.Add(new PlayerItemInventoryService(service));
        saveLoadRemotes.Add(new PlayerHeroItemInventoryService(service));
        saveLoadRemotes.Add(characterService);
        LoadGameData();
    }
    private void LoadGameData()
    {
        int total = saveLoadRemotes.Count;
        int completed = 0;

        foreach (var item in saveLoadRemotes)
        {
            item.LoadGame(gameData, () =>
            {
                completed++;
                if (completed == total)
                {
                    OnLoadGameFormPlayfab?.Invoke(this.gameData);
                }
            });
        }
    }

    public void SaveGameData()
    {

        foreach (var item in saveLoadRemotes)
        {
            item.SaveGame(gameData);
        }
    }
}