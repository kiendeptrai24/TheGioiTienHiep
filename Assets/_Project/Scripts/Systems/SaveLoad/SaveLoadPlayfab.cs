
using System;
using System.Collections.Generic;
using System.Linq;
using TGTH.Mobile;
using UnityEngine;

public class SaveLoadPlayfab : TGTHMonoBehaviour, ISaveManager
{
    public PlayfabDataManager playfabDataManager;
    private GameData gameData = new GameData();
    private List<ISaveable> saveManagers = new List<ISaveable>();
    public event Action<GameData> OnDataReadyToLoad;
    protected override void Awake()
    {
        base.Awake();
        playfabDataManager = PlayfabDataManager.Instance;
        playfabDataManager.OnLoadGameFormPlayfab += OnItemPlayerLoad;
        saveManagers = FindAllSaveManagers();
    }

    private void OnItemPlayerLoad(GameData gameData)
    {
        this.gameData = gameData;
        OnDataReadyToLoad?.Invoke(this.gameData);
    }

    public void NewGame()
    {
        gameData = new GameData();
    }


    public void LoadGame()
    {
        if (this.gameData == null)
        {
            Debug.Log("No saved data found!");
            NewGame();
        }

        foreach (ISaveable saveManager in saveManagers)
        {
            saveManager.LoadData(gameData);
        }
    }
    [ContextMenu("Save Game")]
    public void SaveGame()
    {
        foreach (ISaveable saveManager in saveManagers)
        {
            saveManager.SaveGame(ref gameData);
        }
        playfabDataManager.SaveGameData();
    }

    public void Register(ISaveable saveManager)
    {
        saveManagers.Add(saveManager);
        saveManager.LoadData(gameData);
    }

    public void Unregister(ISaveable saveManager)
    {
        saveManagers.Remove(saveManager);
    }

    private List<ISaveable> FindAllSaveManagers()
    {
        IEnumerable<ISaveable> saveManagers = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ISaveable>();
        return new List<ISaveable>(saveManagers);
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
    }
}