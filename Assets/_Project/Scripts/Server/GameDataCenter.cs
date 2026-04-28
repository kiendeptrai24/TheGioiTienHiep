

using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using Newtonsoft.Json;
using System;

public class GameDataCenter : TGTHNetworkBehaviour
{
    [SerializeField] private List<ItemData> allItems;
    [SerializeField] private List<ItemData> shopItems;
    [SerializeField] private GameDataServer gameDatas;
    private PlayFabDataServerService service;
    private PlayFabClientInstanceAPI clientApi;
    private List<ILoadRemoteServer> saveLoadRemotes = new List<ILoadRemoteServer>();
    [SerializeField] private List<EquitmentData> equipmentDatas;
    [SerializeField] private List<SkillData> skillDatas;
    [SerializeField] private List<TechniqueData> techniqueDatas;
    [SerializeField] private List<RealmData> realmDatas;
    [SerializeField] private List<RaceData> raceDatas;
    [SerializeField] private List<EssenceData> essenceDatas;
    [SerializeField] private List<HeroData> heroDatas;

    public event Action<GameDataServer> OnLoadGameFormPlayfab;

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
        service = new PlayFabDataServerService(clientApi);
        saveLoadRemotes.Add(new InventoryService(service));
        saveLoadRemotes.Add(new RealmService(service));
        saveLoadRemotes.Add(new EssenceAndRaceService(service));
        saveLoadRemotes.Add(new ChampionService(service));
        saveLoadRemotes.Add(new ShopService(service));
        LoadGameData();
    }
    private void LoadDataTest()
    {

        foreach (var item in gameDatas.allItems)
        {
            if (item is EquitmentData)
            {
                equipmentDatas.Add(item as EquitmentData);
            }
            else if (item is SkillData)
            {
                skillDatas.Add(item as SkillData);
            }
            else if (item is TechniqueData)
            {
                techniqueDatas.Add(item as TechniqueData);
            }
        }
        foreach (var item in gameDatas.realmItems)
        {
            realmDatas.Add(item as RealmData);
        }
        foreach (var item in gameDatas.raceAndEssenceItems)
        {
            if(item is RaceData)
                raceDatas.Add(item as RaceData);
            else if(item is EssenceData)
                essenceDatas.Add(item as EssenceData);
        }
        foreach (var item in gameDatas.championItems)
        {
            heroDatas.Add(item as HeroData);
        }
    }
    private void LoadGameData()
    {
        int total = saveLoadRemotes.Count;
        int completed = 0;

        foreach (var item in saveLoadRemotes)
        {
            item.LoadGame(gameDatas, () =>
            {
                completed++;
                if (completed == total)
                {
                    LoadDataTest();
                    OnLoadGameFormPlayfab?.Invoke(this.gameDatas);
                }
            });
        }
    }
}