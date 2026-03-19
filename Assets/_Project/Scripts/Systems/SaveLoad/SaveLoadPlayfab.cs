
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveLoadPlayfab : TGTHMonoBehaviour, ISaveManager
{
    public PlayFabLogin playFabLogin;
    private GameData gameData = new GameData();
    private List<ISaveable> saveManagers = new List<ISaveable>();
    public event Action<GameData> OnDataReadyToLoad;
    protected override void Awake()
    {
        base.Awake();
        playFabLogin.OnLoadGameFormPlayfab += OnItemPlayerLoad;
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

    private void SaveDataFormGame()
    {
        playFabLogin.SetData(gameData);
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

    public void SaveGame()
    {
        foreach (ISaveable saveManager in saveManagers)
        {
            saveManager.SaveGame(ref gameData);
        }
        //SaveDataFormGame();
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
        IEnumerable<ISaveable> saveManagers = Resources.FindObjectsOfTypeAll<MonoBehaviour>().OfType<ISaveable>();
        return new List<ISaveable>(saveManagers);
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
        playFabLogin = FindAnyObjectByType<PlayFabLogin>();
    }
}