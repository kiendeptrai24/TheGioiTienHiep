

using System;

public interface ISaveManager {
    public event Action<GameData> OnDataReadyToLoad;
    void LoadGame();
    void SaveGame();
    void Register(ISaveable saveManager);
    void Unregister(ISaveable saveManager);
}