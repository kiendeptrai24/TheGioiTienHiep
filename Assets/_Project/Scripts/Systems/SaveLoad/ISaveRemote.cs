

using System;

public interface ISaveRemote<TData>
{
    void SaveGame(TData gameData, System.Action<bool> onCompleted = null);
}
