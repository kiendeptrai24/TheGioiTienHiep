

using System;

public interface ISaveLoadRemote
{
    void LoadGame(GameData gameData, Action callback);
    void SaveGame(GameData gameData);
}