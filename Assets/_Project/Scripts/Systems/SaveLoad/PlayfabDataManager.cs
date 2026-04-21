
using System;
using System.Collections.Generic;
using PlayFab;
using UnityEngine;

public class PlayfabDataManager : Singleton<PlayfabDataManager>
{
    public event Action<GameData> OnLoadGameFormPlayfab;
    public event Action<List<ItemData>> OnLoadCharacterFormPlayfab;
    public event Action<List<ItemData>> OnCharacterChanged;
    public event Action<AuthResult> LoginSuccess;
    public event Action<AuthError> LoginError;
    [SerializeField] private GameData gameData = new GameData();
    private List<ISaveLoadRemote> saveLoadRemotes = new List<ISaveLoadRemote>();
    private AuthManager authManager;
    private PlayFabDataService service;
    public List<ItemData> GetCharactersData() => gameData.itemDatasCharacter;
    private ISaveLoadRemote characterService;
    private PlayFabClientInstanceAPI clientAPI;
    public AuthManager GetAuthManager() => authManager;
    public PlayFabClientInstanceAPI GetClientAPI() => clientAPI;
    public ActionNavigationSpecificScreen navigationToCharacterSelectionScreen;
    public bool ready = false;
    protected override void Awake()
    {
        base.Awake();
        navigationToCharacterSelectionScreen = GetComponent<ActionNavigationSpecificScreen>();

        if (Configuration.Instance.buildType == BuildType.LOCAL_SERVER ||
         Configuration.Instance.buildType == BuildType.REMOTE_SERVER) return;

        clientAPI = new PlayFabClientInstanceAPI(PlayFabSettings.staticSettings);

        if (Configuration.Instance.buildType == BuildType.LOCAL_CLIENT)
        {
            IAuthService authService = new PlayFabAuthCustomService(clientAPI);
            authManager = new AuthManager(authService);
            ready = true;
        }
        else if (Configuration.Instance.buildType == BuildType.REMOTE_CLIENT)
        {
            IAuthService authService = new PlayFabAuthService(clientAPI);
            authManager = new AuthManager(authService);
            LobbyController.Instance.OnLobbySearchLobbiesCompleted += (success, lobby) =>
            {
                if (success)
                {
                    if (LobbyController.Instance.HasLobby()) return;
                    LobbyController.Instance.JoinLobby(clientAPI.authenticationContext, lobby.ConnectionString);
                    ready = true;
                }
                else
                {
                    if (LobbyController.Instance.HasLobby()) return;
                    var playfabConnectMutiplayer = new PlayfabConnectMutiplayer(clientAPI.authenticationContext);
                    playfabConnectMutiplayer.RequestMultiplayerServer(clientAPI, Configuration.Instance, result =>
                    {
                        if (result.success)
                        {
                            LobbyController.Instance.CreateLobby(clientAPI.authenticationContext, result.ipAddress, result.port);
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
    protected override void Start()
    {
        base.Start();
        if (Configuration.Instance.buildType == BuildType.LOCAL_SERVER ||
         Configuration.Instance.buildType == BuildType.REMOTE_SERVER) return;
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
        service = new PlayFabDataService(result.clientApi);

        characterService = new ItemCharacterService(service);

        characterService.LoadGame(gameData, () =>
        {
            OnLoadCharacterFormPlayfab?.Invoke(this.gameData.itemDatasCharacter);
        });

        FindRemoteServer();
        LoginSuccess?.Invoke(result);
    }
    private void FindRemoteServer()
    {
        LobbyController.Instance.GetLobbyServer(clientAPI.authenticationContext);

    }
    public void AddCharacter(ItemData itemCharacter)
    {
        var heroData = itemCharacter as HeroData;

        gameData.characterName = itemCharacter.itemName;
        gameData.characterId = heroData.characterId;
        gameData.itemDatas.Add(itemCharacter);
        gameData.itemDatasCharacter.Add(itemCharacter);
        OnCharacterChanged?.Invoke(gameData.itemDatasCharacter);

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
        saveLoadRemotes.Add(new ShopService(service));
        // saveLoadRemotes.Add(new InventoryService(service));
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