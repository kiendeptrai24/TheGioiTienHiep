
using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadOS : ISaveManager
{
    private GameData gameData;
    private List<ISaveable> saveables = new List<ISaveable>();

    public event Action<GameData> OnDataReadyToLoad;

    public SaveLoadOS(GameData gameData, List<ISaveable> saveables)
    {
        this.gameData = gameData;
        this.saveables = saveables;
    }
    public void Register(ISaveable saveManager)
    {
        saveables.Add(saveManager);
        saveManager.LoadData(gameData);
    }
    public void Unregister(ISaveable saveManager)
    {
        saveManager.SaveGame(ref gameData);
        saveables.Remove(saveManager);
    }

    public void LoadGame()
    {
        if (this.gameData == null)
        {
            Debug.Log("No saved data found!");
        }
        foreach (ISaveable saveManager in saveables)
        {
            saveManager.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        foreach (ISaveable saveManager in saveables)
        {
            saveManager.SaveGame(ref gameData);
        }
    }
}