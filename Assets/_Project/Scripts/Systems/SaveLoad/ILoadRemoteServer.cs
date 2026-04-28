

using System;

public interface ILoadRemoteServer
{
    void LoadGame(GameDataCenter gameData, Action callback);
}