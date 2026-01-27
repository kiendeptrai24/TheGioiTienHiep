

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveLoadManager : Singleton<SaveLoadManager>
{
    public ISaveManager saveManager;
    [SerializeField] private StatsCultivationPathPreset statsCultivationPathPreset;
    [SerializeField] private StatsRacePreset statsRacePreset;
    [SerializeField] private StatsRealmPreset statsRealmPreset;
    [SerializeField] private List<ItemPreset> listItemPreset;
    private GameData gameData;

    protected override void Awake()
    {
        base.Awake();
        SetupData();
        saveManager = new SaveLoadOS(gameData, FindAllSaveManagers());
    }
    protected override void Start()
    {
        base.Start();
        saveManager.LoadGame();
    }
    private void SetupData()
    {
        FindAllSaveManagers();
        LoadOSToGameData();
    }
    private List<ISaveable> FindAllSaveManagers()
    {
        IEnumerable<ISaveable> saveManagers = Resources.FindObjectsOfTypeAll<MonoBehaviour>().OfType<ISaveable>();

        return new List<ISaveable>(saveManagers);
    }
    private void LoadOSToGameData()
    {
        gameData = new GameData();
        if (statsCultivationPathPreset != null)
        {
            gameData.statsCultivationPathData = statsCultivationPathPreset.GetStats();
            gameData.statsRaceData = statsRacePreset.GetStats();
            gameData.statsRealmData = statsRealmPreset.GetStats();
        }
        foreach (var itemData in listItemPreset)
        {
            gameData.itemDatas.Add(itemData.GetItemData());
        }
    }
    new private void OnDestroy()
    {
        saveManager.SaveGame();
    }
}