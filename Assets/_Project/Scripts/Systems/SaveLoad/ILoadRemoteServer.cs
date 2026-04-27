

using System;

public interface ILoadRemoteServer
{
    void LoadGame(GameDataServer gameData, Action callback);
}