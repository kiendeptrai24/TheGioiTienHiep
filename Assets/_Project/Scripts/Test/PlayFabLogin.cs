using PlayFab;
using PlayFab.ClientModels;
using PlayFab.DataModels;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using ExitGames.Client.Photon.StructWrapping;

[Serializable]
public class ItemTest
{
    public List<ItemData> items = new List<ItemData>();
}
public class PlayFabLogin : MonoBehaviour
{
    PlayFabPlayer player = new PlayFabPlayer();
    PlayFabClientInstanceAPI clientApi;
    public ItemTest itemsData;
    public ItemTest itemsShop;
    public SkillData itemdad;
    public string playerLoginId = "testLogin1";

    // Start is called before the first frame update
    void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            // Please change the titleId below to your own titleId from PlayFab Game Manager.
            PlayFabSettings.staticSettings.TitleId = "";
        }

        player.Login(playerLoginId, (clientApi) =>
        {
            this.clientApi = clientApi;
            UpdateDisplayName();
        });
    }

    [ContextMenu("Set Data")]
    public void SetData()
    {
        player.SetData(itemsData);
    }
    [ContextMenu("Get Data")]
    public async void GetData()
    {
        player.LoadPlayerData((gameData) =>
        {
            itemsData = gameData;

            var iconLoader = AddressableLoader.Instance.GetLoader<IconLoader>(AddressableLoaderType.Sprite.ToString());
            var prefabLoader = AddressableLoader.Instance.GetLoader<PrefabLoader>(AddressableLoaderType.Prefab.ToString());
            var SODataBase = ScriptableObjectLoader.Instance;

            for (int i = 0; i < itemsData.items.Count; i++)
            {
                var item = itemsData.items[i];
                var itemData = SODataBase.GetItem(item.itemId);

                var sprite = iconLoader.Get(item.itemIconPath);
                itemData.itemIcon = sprite;

                if (itemData is HeroData heroData)
                {
                    var heroPrefab = prefabLoader.Get(itemData.itemFilePath);
                    heroData.heroPrefab = heroPrefab;

                    for (int h = 0; h < heroData.skillDatas.Count; h++)
                    {
                        var skill = heroData.skillDatas[h];

                        var skillData = SODataBase.GetItem(skill.itemId) as SkillData;
                        SetSkilldata(iconLoader, prefabLoader, h, skillData);
                        heroData.skillDatas[h] = skillData;
                        itemdad = heroData.skillDatas[h];
                    }

                    for (int s = 0; s < heroData.techniqueDatas.Count; s++)
                    {
                        var technique = heroData.techniqueDatas[s];
                        var techniqueData = SODataBase.GetItem(technique.itemId) as TechniqueData;

                        heroData.techniqueDatas[s] = techniqueData;
                    }

                    itemsData.items[i] = heroData;
                    continue;
                }

                if (itemData is SkillData skillDatas)
                {
                    SetSkilldata(iconLoader, prefabLoader, i, skillDatas);
                    continue;
                }

                itemsData.items[i] = itemData;
            }
        });
    }
    [ContextMenu("Get Shop Data")]
    public async void GetShopData()
    {
        player.LoadShopData((gameData) =>
        {
            itemsShop = gameData;

            var iconLoader = AddressableLoader.Instance.GetLoader<IconLoader>(AddressableLoaderType.Sprite.ToString());
            var prefabLoader = AddressableLoader.Instance.GetLoader<PrefabLoader>(AddressableLoaderType.Prefab.ToString());
            var SODataBase = ScriptableObjectLoader.Instance;

            for (int i = 0; i < itemsShop.items.Count; i++)
            {
                var item = itemsShop.items[i];
                var itemData = SODataBase.GetItem(item.itemId);

                var sprite = iconLoader.Get(item.itemIconPath);
                itemData.itemIcon = sprite;

                if (itemData is HeroData heroData)
                {
                    var heroPrefab = prefabLoader.Get(itemData.itemFilePath);
                    heroData.heroPrefab = heroPrefab;

                    for (int h = 0; h < heroData.skillDatas.Count; h++)
                    {
                        var skill = heroData.skillDatas[h];

                        var skillData = SODataBase.GetItem(skill.itemId) as SkillData;
                        SetSkillShopdata(iconLoader, prefabLoader, h, skillData);
                        heroData.skillDatas[h] = skillData;
                        itemdad = heroData.skillDatas[h];
                    }

                    for (int s = 0; s < heroData.techniqueDatas.Count; s++)
                    {
                        var technique = heroData.techniqueDatas[s];
                        var techniqueData = SODataBase.GetItem(technique.itemId) as TechniqueData;

                        heroData.techniqueDatas[s] = techniqueData;
                    }

                    itemsShop.items[i] = heroData;
                    continue;
                }

                if (itemData is SkillData skillDatas)
                {
                    SetSkillShopdata(iconLoader, prefabLoader, i, skillDatas);
                    continue;
                }

                itemsShop.items[i] = itemData;
            }
        });
    }

    private void SetSkilldata(IconLoader iconLoader, PrefabLoader prefabLoader, int i, SkillData skillDatas)
    {
        skillDatas.itemIcon = iconLoader.Get(skillDatas.itemIconPath);
        skillDatas.skillEffectPrefab = prefabLoader.Get(skillDatas.itemFilePath);

        itemsData.items[i] = skillDatas;
    }
    private void SetSkillShopdata(IconLoader iconLoader, PrefabLoader prefabLoader, int i, SkillData skillDatas)
    {
        skillDatas.itemIcon = iconLoader.Get(skillDatas.itemIconPath);
        skillDatas.skillEffectPrefab = prefabLoader.Get(skillDatas.itemFilePath);

        itemsShop.items[i] = skillDatas;
    }
    private void UpdateDisplayName()
    {
        clientApi.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = "Kiên ngô"
        }, result =>
        {
            Debug.Log("The player's display name is now: " + result.DisplayName);
        }, error => Debug.LogError(error.GenerateErrorReport()));
    }

    [ContextMenu("Get Player Profile")]
    public void GetPlayerProfile()
    {
        clientApi.GetPlayerProfile(new GetPlayerProfileRequest()
        {
            PlayFabId = playerLoginId,
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true
            }
        },
        result =>
        {

            Debug.Log("The player's DisplayName profile data is: " + result.PlayerProfile.DisplayName);
        },
        error => Debug.LogError(error.GenerateErrorReport()));
    }
}

class PlayFabPlayer
{
    public bool loggedIn = false;
    public bool dataLoading = false;
    public bool dataLoaded = false;
    public string PlayFabId;
    public Dictionary<string, ObjectResult> playerData;
    public Dictionary<string, UserDataRecord> data;

    private PlayFabClientInstanceAPI clientApi;
    private PlayFabDataInstanceAPI dataApi;

    public void Login(string customId, Action<PlayFabClientInstanceAPI> callback)
    {
        clientApi = new PlayFabClientInstanceAPI(PlayFabSettings.staticSettings);

        var request = new LoginWithCustomIDRequest { CustomId = customId, CreateAccount = true };

        clientApi.LoginWithCustomID(request, result =>
        {
            PlayFabId = result.PlayFabId;
            loggedIn = true;
            dataApi = new PlayFabDataInstanceAPI(clientApi.authenticationContext);
            callback?.Invoke(clientApi);
            Debug.Log("Login call succeeded.");
        }, error =>
        {
            Debug.LogWarning("Something went wrong with the login call.");
            Debug.LogError("Here's some debug information:");
            Debug.LogError(error.GenerateErrorReport());
        });
    }
    public void LoadData(Action<ItemTest> callback)
    {
        clientApi.GetTitleData(new GetTitleDataRequest(),
        r =>
        {
            if (r.Data != null && r.Data.ContainsKey("inventory"))
            {
                string json = r.Data["inventory"];

                Debug.Log(json);

                ItemTest item = JsonConvert.DeserializeObject<ItemTest>(json);

                callback?.Invoke(item);

                Debug.Log(item.items.Count);
                Debug.Log("Load success");
            }
        },
        error =>
        {
            Debug.LogError(error.GenerateErrorReport());
        });
    }
    public void LoadShopData(Action<ItemTest> callback)
    {
        clientApi.GetTitleData(new GetTitleDataRequest(),
        r =>
        {
            if (r.Data != null && r.Data.ContainsKey("shop"))
            {
                string json = r.Data["shop"];

                Debug.Log(json);

                ItemTest item = JsonConvert.DeserializeObject<ItemTest>(json);

                callback?.Invoke(item);

                Debug.Log(item.items.Count);
                Debug.Log("Load success");
            }
        },
        error =>
        {
            Debug.LogError(error.GenerateErrorReport());
        });
    }
    public void LoadPlayerData(Action<ItemTest> callback)
    {
        clientApi.GetUserData(new GetUserDataRequest(),
        result =>
        {
            if (result.Data != null)
            {
                var r = result;
                if (r.Data != null && r.Data.ContainsKey("inventory"))
                {
                    string json = r.Data["inventory"].Value;

                    Debug.Log(json);

                    ItemTest item = JsonConvert.DeserializeObject<ItemTest>(json);
                    callback?.Invoke(item);
                    Debug.Log("Load success");
                }
            }
            else
            {
                Debug.Log("Không có player data");
            }
        },
        error =>
        {
            Debug.LogError(error.GenerateErrorReport());
        });
    }



    public void SetData(ItemTest items)
    {
        string key = "inventory";

        string json = JsonConvert.SerializeObject(items);
        clientApi.UpdateUserData(new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string> { { key, json } }
        }, r => Debug.Log("Set success"), e => Debug.LogError(e.GenerateErrorReport()));
    }
}