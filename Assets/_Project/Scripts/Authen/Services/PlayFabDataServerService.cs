using System;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayFabDataServerService
{
    private readonly PlayFabClientInstanceAPI clientApi;

    public PlayFabDataServerService(PlayFabClientInstanceAPI clientApi)
    {
        this.clientApi = clientApi;
    }

    #region Public Load Methods

    public void LoadData(Action<ItemInventoryResponseDto> callback)
    {
        LoadTitleData("inventory", callback);
    }
    public void LoadRealmData(Action<ItemRealmResponseDto> callback)
    {
        LoadTitleData("realm", callback);
    }
    public void LoadEssenceAndRaceData(Action<ItemEssenceAndRaceResponseDto> callback)
    {
        LoadTitleData("essenceAndRace", callback);
    }
    public void LoadChampionData(Action<ItemRealmResponseDto> callback)
    {
        LoadTitleData("champion", callback);
    }
    
    public void LoadShopData(Action<ItemDataDTO> callback)
    {
        LoadTitleData("shop", callback);
    }
    #endregion

    #region Private Generic Load/Save

    private void LoadTitleData<T>(string key, Action<T> callback)
    {
        clientApi.GetTitleData(
            new GetTitleDataRequest(),
            result =>
            {
                if (result.Data == null)
                {
                    Debug.LogWarning($"LoadTitleData<{typeof(T).Name}> failed: TitleData is null");
                    callback?.Invoke(default);
                    return;
                }

                if (!result.Data.TryGetValue(key, out string json) || string.IsNullOrEmpty(json))
                {
                    Debug.LogWarning($"LoadTitleData<{typeof(T).Name}> failed: key '{key}' not found");
                    callback?.Invoke(default);
                    return;
                }

                TryDeserialize(json, callback, key);
            },
            error =>
            {
                Debug.LogError($"GetTitleData Error: {error.GenerateErrorReport()}");
                callback?.Invoke(default);
            });
    }

    private void TryDeserialize<T>(string json, Action<T> callback, string key)
    {
        try
        {
            T data = JsonConvert.DeserializeObject<T>(json);
            callback?.Invoke(data);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Deserialize Error at key '{key}': {ex}");
            callback?.Invoke(default);
        }
    }

    #endregion
}