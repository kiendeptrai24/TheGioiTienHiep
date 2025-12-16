public interface ISaveable
{
    void LoadData(GameData _data);
    void SaveGame(ref GameData _data);
}