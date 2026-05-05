

using System;

public interface ILoadRemote<T>
{
    void LoadGame(T gameData, Action callback);
}