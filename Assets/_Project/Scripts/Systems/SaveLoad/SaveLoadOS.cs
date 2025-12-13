
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveLoadOS : MonoBehaviour
{
    [SerializeField] private StatsCultivationPathPreset statsCultivationPathPreset;
    [SerializeField] private StatsRacePreset statsRacePreset;
    [SerializeField] private StatsRealmPreset statsRealmPreset;
    [SerializeField] private List<ItemPreset> listItemPreset;
    private GameData gameData;
    private List<ISaveManager> saveManagers = new List<ISaveManager>();


    private void Awake() 
    {
        LoadOSToGameData();
    }

    private void Start() {
        saveManagers = FindAllSaveManagers();
        LoadGame();
    }
    public void LoadGame()
    {
        LoadOSToGameData();
        if(this.gameData == null)
        {
            Debug.Log("No saved data found!");
        }

        foreach (ISaveManager saveManager in saveManagers)
        {
            saveManager.LoadData(gameData);
        }
    }
    
    public void SaveGame()
    {
        foreach(ISaveManager saveManager in saveManagers)
        {
            saveManager.SaveGame(ref gameData);
        }
    }
    private void OnApplicationQuit() 
    {
        //SaveGame();
    }
    private List<ISaveManager> FindAllSaveManagers()
    {
        IEnumerable<ISaveManager> saveManagers = Resources.FindObjectsOfTypeAll<MonoBehaviour>().OfType<ISaveManager>();
        
        return new List<ISaveManager>(saveManagers);
    }

    private void LoadOSToGameData()
    {
        gameData = new GameData();
        gameData.statsCultivationPathData = statsCultivationPathPreset.GetStats();
        gameData.statsRaceData = statsRacePreset.GetStats();
        gameData.statsRealmData = statsRealmPreset.GetStats();
        foreach (var itemData in listItemPreset)
        {
            gameData.itemDatas.Add(itemData.GetItemData());
        }
    }
}