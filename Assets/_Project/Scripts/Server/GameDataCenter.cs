

using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using Newtonsoft.Json;

public class GameDataCenter : TGTHNetworkBehaviour
{
    [SerializeField] private List<ItemData> allItems;
    [SerializeField] private List<ItemData> shopItems;
    [SerializeField] private GameData gameDatas;
    private PlayFabDataService service;
    private PlayFabClientInstanceAPI clientApi;
    private List<ISaveLoadRemote> saveLoadRemotes = new List<ISaveLoadRemote>();
    [SerializeField] private List<EquitmentData> equipmentDatas;
    [SerializeField] private List<SkillData> skillDatas;
    [SerializeField] private List<TechniqueData> techniqueDatas;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer) return;
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
        service = new PlayFabDataService(clientApi);
        LoadData();
    }
    private void LoadData()
    {
        saveLoadRemotes.Add(new InventoryService(service));
        foreach (var load in saveLoadRemotes)
        {
            load.LoadGame(gameDatas, () =>
            {
                foreach (var item in gameDatas.allItemsDatas)
                {
                    if(item is EquitmentData)
                    {
                        equipmentDatas.Add(item as EquitmentData);
                    }
                    else if(item is SkillData)
                    {
                        skillDatas.Add(item as SkillData);
                    }
                    else if(item is TechniqueData)
                    {
                        techniqueDatas.Add(item as TechniqueData);
                    }
                }
            });
        }
    }
}