
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadManager : Singleton<SaveLoadManager>
{
    public ISaveManager saveManager;
    public List<HeroData> heroDatas = new List<HeroData>();
    [SerializeField] private GameData gameData;
    protected override void Awake()
    {
        base.Awake();
        saveManager = GetComponent<ISaveManager>();
        saveManager.OnDataReadyToLoad += OnItemPlayerLoad;
    }

    private void OnItemPlayerLoad(GameData data)
    {
        gameData = data;
        saveManager.LoadGame();
        foreach (var item in gameData.itemCharacterDatas)
        {
            heroDatas.Add(item as HeroData);
        }
    }
    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
        SaveGame();
    }
    public void SaveGame()
    {
        if (gameData == null) return;
        saveManager.SaveGame();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause) SaveGame();
    }
    private new void OnDestroy()
    {
        if (saveManager != null)
            saveManager.OnDataReadyToLoad -= OnItemPlayerLoad;
    }
}