using PlayFab;
using PlayFab.ClientModels;
using PlayFab.DataModels;
using PlayFab.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;

[Serializable]
public class ItemTest
{
    public List<ItemData> items = new List<ItemData>();
}

public class PlayFabLogin : MonoBehaviour
{
    PlayFabPlayer player1 = new PlayFabPlayer();
    public List<ItemPreset> presets;
    public ItemTest itemsData;

    // Start is called before the first frame update
    void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            // Please change the titleId below to your own titleId from PlayFab Game Manager.
            PlayFabSettings.staticSettings.TitleId = "";
        }

        player1.Login("testLogin1");
    }

    [ContextMenu("Set Data")]
    public void SetData()
    {
        player1.SetData(new ItemTest());
    }
    [ContextMenu("Get Data")]
    public void GetData()
    {
        player1.LoadData((gameData) =>
        {
            itemsData = gameData;
            foreach (var item in itemsData.items)
            {
                Sprite icon = Resources.Load<Sprite>(item.itemIconPath);
                item.itemIcon = icon;
            }

        });
    }
    [ContextMenu("To Json")]
    public void ToJson()
    {
        List<ItemData> items = new List<ItemData>();
        foreach (var item in presets)
        {
            items.Add(item.GetItemData());
        }
        ItemJsonCreator.CreateItemJson(items);
    }
    [ContextMenu("Load item preset")]
    public void LoadItemsPreset()
    {
        presets = ItemPresetLoader.GetAllItemPresets();
    }
}
public static class ItemJsonCreator
{
    public static void CreateItemJson(List<ItemData> itemList)
    {
        ItemTest itemTest = new ItemTest();
        itemTest.items = itemList;
        string json = JsonConvert.SerializeObject(itemTest);

        string path = Application.dataPath + "/item.json";

        File.WriteAllText(path, json);

        Debug.Log("JSON created at: " + path);
        Debug.Log(json);
    }
}
public static class ItemPresetLoader
{
    public static List<ItemPreset> GetAllItemPresets()
    {
        List<ItemPreset> items = new List<ItemPreset>();

        string[] guids = AssetDatabase.FindAssets("t:ItemPreset",
            new[] { "Assets/_Project/Data/OS" });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ItemPreset item = AssetDatabase.LoadAssetAtPath<ItemPreset>(path);

            if (item != null)
                items.Add(item);
        }

        return items;
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

    public void Login(string customId)
    {
        clientApi = new PlayFabClientInstanceAPI(PlayFabSettings.staticSettings);

        var request = new LoginWithCustomIDRequest { CustomId = customId, CreateAccount = true };

        clientApi.LoginWithCustomID(request, result =>
        {
            PlayFabId = result.PlayFabId;
            loggedIn = true;
            dataApi = new PlayFabDataInstanceAPI(clientApi.authenticationContext);
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
    public void SetData(ItemTest items)
    {
        string key = "Inventory";

        string json = JsonUtility.ToJson(items);
        clientApi.UpdateUserData(new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string> { { key, json } }
        }, r => Debug.Log("Set success"), e => Debug.LogError(e.GenerateErrorReport()));
    }
}