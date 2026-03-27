
using System;
using System.Collections.Generic;
using System.Linq;
using PlayFab;
using Unity.Netcode;
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
    protected override void Awake()
    {
        base.Awake();
        clientAPI = new PlayFabClientInstanceAPI(PlayFabSettings.staticSettings);
        IAuthService authService = new PlayFabAuthCustomService(clientAPI);
        authManager = new AuthManager(authService);
    }
    protected override void Start()
    {
        base.Start();
        authManager.AutoLogin(onSuccess, onError);
    }
    public void Login(LoginData loginData)
    {
        authManager.Login(loginData, onSuccess, onError);
    }
    public void Logout()
    {
        authManager.Logout(default, default);
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
        LoginSuccess?.Invoke(result);
    }
    public void AddCharacter(ItemData itemCharacter)
    {
        var heroData = itemCharacter as HeroData;

        gameData.characterName = itemCharacter.itemName;
        gameData.characterId = heroData.characterId;
        gameData.itemDatas.Add(itemCharacter);
        gameData.itemDatasCharacter.Add(itemCharacter);
        OnCharacterChanged?.Invoke(gameData.itemDatasCharacter);

        var playerInventoryService = new PlayerItemInventoryService(service);

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
        saveLoadRemotes.Add(new InventoryService(service));
        saveLoadRemotes.Add(new TeamInventoryService(service));
        saveLoadRemotes.Add(new PlayerItemInventoryService(service));
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