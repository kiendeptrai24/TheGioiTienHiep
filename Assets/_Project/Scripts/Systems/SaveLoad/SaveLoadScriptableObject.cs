
using System;
using System.Collections.Generic;
using System.Linq;
using TGTH.Mobile;
using UnityEngine;

public class SaveLoadScriptableObject : TGTHMonoBehaviour, ISaveManager
{
    private GameData gameData = new GameData();
    [SerializeField] private List<ItemPreset> itemPresets;
    private List<ISaveable> saveManagers = new List<ISaveable>();
    public event Action<GameData> OnDataReadyToLoad;
    protected override void Awake()
    {
        base.Awake();
        saveManagers = FindAllSaveManagers();
    }
    protected override void Start()
    {
        base.Start();
        InitData();
    }
    private void InitData()
    {
        var temp = new List<ItemData>();
        foreach (var itemPreset in itemPresets)
        {
            var itemData = itemPreset.GetItemData();
            temp.Add(itemData);
        }
        gameData.itemCharacterDatas = new();
        gameData.itemInTeamDatas = new();

        gameData.allItemsDatas = temp;
        gameData.itemDatas = temp;
        gameData.itemShopDatas = temp;
        OnDataReadyToLoad?.Invoke(gameData);
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