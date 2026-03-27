

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveLoadManager : Singleton<SaveLoadManager>
{
    public ISaveManager saveManager;
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
    }
    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
        saveManager.SaveGame();
    }
    private new void OnDestroy()
    {
        if (saveManager != null)
            saveManager.OnDataReadyToLoad -= OnItemPlayerLoad;
    }
}