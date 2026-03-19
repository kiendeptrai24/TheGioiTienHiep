
using System;
using UnityEngine;

public class ProfileService : ISaveLoadRemote
{
    private PlayFabLogin playFabLogin;
    public ProfileService(PlayFabLogin playFabLogin)
    {
        this.playFabLogin = playFabLogin;
    }

    public void LoadGame(GameData gameData, Action callback)
    {
        playFabLogin.player.LoadProfile((profileDataDTO) =>
        {
            gameData.playerName = profileDataDTO.playerName;
            gameData.coins = profileDataDTO.coins;
            callback?.Invoke();
        });
    }

    public void SaveGame(GameData gameData)
    {
        playFabLogin.player.SetProfile(gameData);
    }
}