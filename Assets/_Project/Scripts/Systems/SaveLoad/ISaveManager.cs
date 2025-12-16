

public interface ISaveManager {
    void LoadGame();
    void SaveGame();
    void Register(ISaveable saveManager);
    void Unregister(ISaveable saveManager);
}