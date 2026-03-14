using PlayFab;
using PlayFab.ClientModels;
using PlayFab.DataModels;
using PlayFab.Internal;
using System.Collections.Generic;
using UnityEngine;

public class PlayFabLogin : MonoBehaviour
{
    PlayFabPlayer player1 = new PlayFabPlayer();
    public ItemPreset itemPreset;
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

    // Update is called once per frame
    void Update()
    {
        // if (player1.loggedIn && !player1.dataLoaded && !player1.dataLoading)
        // {
        //     player1.LoadData();
        // }
    }
    [ContextMenu("Set Data")]
    public void SetData()
    {
        var item = itemPreset.GetItemData();
        player1.SetData(item);
    }
    [ContextMenu("Get Data")]
    public void GetData()
    {
        player1.LoadData();
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
    public void LoadData()
    {
        clientApi.GetUserData(new GetUserDataRequest(),
        r =>
        {
            data = r.Data;
            if (r.Data != null && r.Data.ContainsKey("Inventory"))
            {
                string json = r.Data["Inventory"].Value;
                ItemData item = JsonUtility.FromJson<ItemData>(json);

                Debug.Log("Inventory loaded: " + item.itemName);
            }
        },
        e => Debug.LogError(e.GenerateErrorReport()));
    }
    public void SetData(ItemData itemData)
    {
        string key = "Inventory";

        string json = JsonUtility.ToJson(itemData);
        clientApi.UpdateUserData(new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string> { { key, json } }
        }, r => Debug.Log("Set success"), e => Debug.LogError(e.GenerateErrorReport()));
    }
}