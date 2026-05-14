
using System;
using System.Collections.Generic;
using System.Linq;
using PlayFab;
using PlayFab.ClientModels;
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

    protected override void Awake()
    {
        base.Awake();
        gameDataCenterManager = GameDataCenterManager.Instance;
        gameDataCenterManager.OnLoadGameDataCenterSuccessed += OnDataCenterReady;
        navigationToCharacterSelectionScreen = GetComponent<ActionNavigationSpecificScreen>();
        Authen();
    }
    public List<ItemData> GetCharactersData() => gameData.itemCharacterDatas;
    private void Authen()
    {
        clientApi = new PlayFabClientInstanceAPI(PlayFabSettings.staticSettings);
        if (Configuration.Instance.IsServerBuild())
        {
            IAuthService authService = new PlayFabAuthCustomService(clientApi, true);
            authFacade = new AuthFacade(authService);
            authFacade.Login(new LoginData(), onSuccess, onError);
        }
        if (Configuration.Instance.IsClientLocalBuild())
        {
            IAuthService authService = new PlayFabAuthCustomService(clientApi);
            authFacade = new AuthFacade(authService);
            ready = true;
        }
        else if (Configuration.Instance.IsClientRemoteBuild())
        {
            IAuthService authService = new PlayFabAuthService(clientApi);
            authFacade = new AuthFacade(authService);
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

            if (data == null)
            {
                return;
            }
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
        authFacade.Logout(onSuccess, onError);
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
        authFacade.AutoLogin(onSuccess, onError);
    }
    public void Login(LoginData loginData)
    {
        authFacade.Login(loginData, onSuccess, onError);
    }
    public void Logout()
    {
        authFacade.Logout((result) =>
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
        gameDataCenterManager.onSuccess(result.clientApi);

        if (Configuration.Instance.IsClientBuild())
        {
            sessionId = result.sessionId;
            CreateSession();
            FindRemoteServer();
            LoginSuccess?.Invoke(result);
            hasLogined = true;
            if (gameDataCenterManager.IsReady() == false) return;
            LoadCharacterDataChoose();
        }
    }

    private void LoadCharacterDataChoose()
    {
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
    }

    private void FindRemoteServer()
    {
        LobbyController.Instance.GetLobbyServer(clientApi.authenticationContext);
    }
    public void AddCharacter(ItemData itemCharacter)
    {
        var heroData = itemCharacter as HeroData;

        if (heroData == null)
        {
            Debug.LogError("AddCharacter failed: itemCharacter is not HeroData");
            return;
        }
        heroData.isCharactor = true;
        gameData.characterName = itemCharacter.itemName;
        gameData.characterId = heroData.characterId;
        gameData.itemDatas.Add(itemCharacter);
        gameData.itemCharacterDatas.Add(itemCharacter);
        OnCharacterChanged?.Invoke(gameData.itemCharacterDatas);

        gameData.potentialPoint = heroData.realmData.rewardPotentialPoint;
        gameData.skillPoint = heroData.realmData.rewardSkillPoint;
        characterService.SaveGame(gameData);
    }
    public void OnCharacterLoaded(string characterId)
    {
        gameData.characterId = characterId;
        LoadGameData();
    }
    private void LoadGameData()
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