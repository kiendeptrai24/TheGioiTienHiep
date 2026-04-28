using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
public enum DataDTOType
{
    ItemDTO,
    HeroDTO,
}
public enum ItemConvertType
{
    AllItem,
    ShopItem,
}
public class ScriptableObjectLoader : Singleton<ScriptableObjectLoader>
{
    public DataDTOType dataDTOType;
    public ItemConvertType itemConvertType;
    public List<ItemPreset> baseItems = new();
    public List<StatsRealmPreset> baseRealmItems = new();
    public List<StatsRacePreset> baseRaceItems = new();
    public List<ItemPreset> baseShopItems = new();
    public List<ItemPreset> testItems = new();
    private Dictionary<string, ItemPreset> items = new();
    private Dictionary<RealmType, StatsRealmPreset> realmItems = new();
    private Dictionary<RaceType, StatsRacePreset> raceItems = new();
    protected override void Awake()
    {
        base.Awake();
        foreach (var item in baseItems)
        {
            if (items.ContainsKey(item.instanceId))
                continue;
            items.Add(item.instanceId, item);
        }
        foreach (var item in baseRealmItems)
        {
            if (realmItems.ContainsKey(item.realmType))
                continue;
            realmItems.Add(item.realmType, item);
        }
        foreach (var item in baseRaceItems)
        {
            if (raceItems.ContainsKey(item.raceType))
                continue;
            raceItems.Add(item.raceType, item);
        }

    }
    public ItemData GetItem(string instanceId)
    {
        if (items.TryGetValue(instanceId, out var item))
            return item.GetItemData();
        Debug.Log("Item not found for instanceId: " + instanceId);
        return null;
    }
    public RealmData GetRealmItem(RealmType realmType)
    {
        if (realmItems.TryGetValue(realmType, out var realmItem))
            return realmItem.GetStats();
        Debug.Log("Realm item not found for realm type: " + realmType);
        return null;
    }
    public RaceData GetRaceItem(RaceType raceType)
    {
        if (raceItems.TryGetValue(raceType, out var raceItem))
            return raceItem.GetStats();
        Debug.Log("race item not found for race type: " + raceType);
        return null;
    }
#if UNITY_EDITOR
    [ContextMenu("Load All Item Presets")]
    public void LoadAllItemPresets()
    {
        string[] guids = AssetDatabase.FindAssets("t:ItemPreset",
            new[] { "Assets/_Project/Data/OS" });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ItemPreset item = AssetDatabase.LoadAssetAtPath<ItemPreset>(path);

            if (item != null)
                baseItems.Add(item);
        }
    }
    [ContextMenu("To Json")]
    public void ToJson()
    {
        var Listtems = baseItems;
        if (itemConvertType == ItemConvertType.ShopItem)
        {
            var itemDataDTO = new ItemDataDTO();
            foreach (var item in baseShopItems)
            {
                itemDataDTO.inventoryItems.Add(item.GetItemData());
            }
            ItemJsonCreator.CreateItemJson(itemDataDTO);
            return;
        }
        if (dataDTOType == DataDTOType.HeroDTO)
        {
            var HeroDataDTO = new HeroInTeamDataDTO();
            foreach (var item in Listtems)
            {
                var itemData = item.GetItemData();
                var heroData = itemData as HeroData;
                HeroDataDTO.inventoryItems.Add(heroData);
                HeroDataDTO.championsIndex.Add(heroData.championIndex);
            }
            ItemJsonCreator.CreateItemJson(HeroDataDTO);
        }
        else
        {
            var itemDataDTO = new ItemDataDTO();
            foreach (var item in Listtems)
            {
                itemDataDTO.inventoryItems.Add(item.GetItemData());
            }
            ItemJsonCreator.CreateItemJson(itemDataDTO);
        }
    }


#endif

}