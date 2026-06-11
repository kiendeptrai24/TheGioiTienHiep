using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChampionListSnapshot : Singleton<ChampionListSnapshot>
{
    [SerializeField] private List<ItemData> championDatasInInventory = new();
    [SerializeField] private List<ItemData> championDatasInTeam = new();
    [SerializeField] private List<ItemData> championDatasInInventoryTemp = new();
    [SerializeField] private Dictionary<ItemData, Vector2Int> championDatasInTeamTemp = new();
    private InventoryCenterManager inventoryCM;
    public event Action OnLoadDataSuccessed;
    public event Action OnDataChanged;
    public event Action OnDataUndo;
    public event Action OnDataSave;
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
        inventoryCM = InventoryCenterManager.Instance;
        inventoryCM.OnLoadDataSuccessed += () =>
        {
            SetUpOrigin();
            OnLoadDataSuccessed?.Invoke();
        };
        inventoryCM.OnItemExistingChampionDataChanged += (item) =>
        {
            SetUpOrigin();
            OnDataChanged?.Invoke();
        };
        SetUpOrigin();
    }
    public List<ItemData> GetDatasChampionInTeam() => championDatasInTeamTemp.Keys.ToList();
    public Dictionary<ItemData, Vector2Int> GetDicDatasChampionInTeam() => championDatasInTeamTemp;
    public List<ItemData> GetDatasChampionInInventory() => championDatasInInventoryTemp.ToList();
    public void SetUpOrigin()
    {
        if (inventoryCM == null) return;
        championDatasInInventory = inventoryCM.GeChampiontDatasExisting();
        championDatasInTeam = inventoryCM.GetDatasChampionInTeam();
        ResetData();
    }

    private void ResetData()
    {
        championDatasInInventoryTemp.Clear();
        championDatasInTeamTemp.Clear();
        foreach (var item in championDatasInInventory)
        {
            if (championDatasInInventoryTemp.Contains(item) == false)
                championDatasInInventoryTemp.Add(item);
        }
        foreach (var item in championDatasInTeam)
        {
            if (championDatasInTeamTemp.ContainsKey(item) == false)
            {
                championDatasInTeamTemp.Add(item, (item as HeroData).championIndex);
            }
        }
    }

    public void Save()
    {
        foreach (var item in championDatasInTeamTemp)
        {
            var cham = item.Key as HeroData;
            cham.championIndex = item.Value;
        }
        inventoryCM.SetItemChampionData(championDatasInTeamTemp.Keys.ToList());
        inventoryCM.SetItemChampionDataExists(championDatasInInventoryTemp);
        SetUpOrigin();
        OnDataSave?.Invoke();
    }

    public void Undo()
    {
        SetUpOrigin();
        OnDataUndo?.Invoke();
    }
    public void EquipData(ItemData item, Vector2Int index)
    {
        if (championDatasInTeamTemp.ContainsKey(item) == false)
        {
            championDatasInTeamTemp.Add(item, index);
        }

        if (championDatasInInventoryTemp.Contains(item))
            championDatasInInventoryTemp.Remove(item);

        OnDataChanged?.Invoke();
    }
    public void UnEquipData(ItemData item)
    {
        if (championDatasInTeamTemp.ContainsKey(item))
        {
            championDatasInTeamTemp.Remove(item);
        }
        if (championDatasInInventoryTemp.Contains(item) == false)
            championDatasInInventoryTemp.Add(item);
        OnDataChanged?.Invoke();
    }
    public void SwapIndex(ItemData itemData, Vector2Int index)
    {
        if (championDatasInTeamTemp.ContainsKey(itemData) == false) return;
        championDatasInTeamTemp[itemData] = index;
    }
}
