
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveLoadJson : MonoBehaviour
{
    [SerializeField] private string fileName;
    [SerializeField] private bool encryptData;
    private GameData gameData;
    private List<ISaveable> saveManagers = new List<ISaveable>();

    private FileDataHandler dataHandler;
    [ContextMenu("Delete save file")]
    public void DeleteSaveData()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);
        dataHandler.Delete();
    }
    private void Awake()
    {

    }
    public void SetUp()
    {
        FindAllSaveManagers();
    }
    private void Start()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);
        saveManagers = FindAllSaveManagers();
        LoadGame();
    }

    public void NewGame()
    {
        gameData = new GameData();
    }

    public void LoadGame()
    {
        gameData = dataHandler.Load();
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
        dataHandler.Save(gameData);
    }
    private void OnApplicationQuit()
    {
        SaveGame();
    }
    private List<ISaveable> FindAllSaveManagers()
    {
        IEnumerable<ISaveable> saveManagers = Resources.FindObjectsOfTypeAll<MonoBehaviour>().OfType<ISaveable>();


        return new List<ISaveable>(saveManagers);
    }



    public bool HadSaveData()
    {

        if (dataHandler.Load() != null)
            return true;
        return false;
    }
}