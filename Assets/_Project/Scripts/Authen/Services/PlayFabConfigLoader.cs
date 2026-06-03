using System;
using UnityEngine;

[Serializable]
public class PlayFabConfigData
{
    public string titleId;
    public string developerSecretKey;
}

public class PlayFabConfigLoader
{
    private static PlayFabConfigData _config;

    public static PlayFabConfigData LoadConfig()
    {
        if (_config != null)
            return _config;

        TextAsset configAsset = Resources.Load<TextAsset>("PlayFabConfig");
        if (configAsset == null)
        {
            Debug.LogError("PlayFabConfig.json not found in Resources folder!");
            return null;
        }

        try
        {
            _config = JsonUtility.FromJson<PlayFabConfigData>(configAsset.text);
            Debug.Log($"PlayFab Config loaded: TitleId={_config.titleId}");
            return _config;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to parse PlayFabConfig.json: {ex}");
            return null;
        }
    }

    public static string GetTitleId()
    {
        var config = LoadConfig();
        return config?.titleId ?? "";
    }

    public static string GetDeveloperSecretKey()
    {
        var config = LoadConfig();
        return config?.developerSecretKey ?? "";
    }
}
