using System;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ServerModels;
using UnityEngine;

public class PlayFabDataServerService
{
    private readonly PlayFabServerInstanceAPI serverApi;

    public PlayFabDataServerService()
    {
        PlayFabSettings.staticSettings.TitleId = PlayFabConfigLoader.GetTitleId();
        PlayFabSettings.staticSettings.DeveloperSecretKey = PlayFabConfigLoader.GetDeveloperSecretKey();
        this.serverApi = new PlayFabServerInstanceAPI(PlayFabSettings.staticSettings);
    }

    #region Public Load Methods
    public void LoadAllGameData(Action<AllGameDataResponseDto> callback)
    {
        LoadTitleData("all_game_data", callback);
    }
    #endregion

    #region Private Generic Load/Save

    private void LoadTitleData<T>(string key, Action<T> callback)
    {
        try
        {
            serverApi.GetTitleData(
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
        catch (System.Exception)
        {

            Debug.LogError($"LoadTitleData<{typeof(T).Name}> failed: Exception occurred while trying to load title data with key '{key}'");
            return;
        }
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