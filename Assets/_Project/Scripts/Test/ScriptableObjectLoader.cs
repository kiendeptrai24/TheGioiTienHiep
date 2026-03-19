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
    public bool TestJson = false;
    public DataDTOType dataDTOType;
    public ItemConvertType itemConvertType;
    public List<ItemPreset> baseItems = new();
    public List<ItemPreset> baseShopItems = new();
    public List<ItemPreset> testItems = new();
    private Dictionary<string, ItemPreset> items = new();
    protected override void Awake()
    {
        base.Awake();
        foreach (var item in baseItems)
        {
            if (items.ContainsKey(item.itemId))
                continue;
            items.Add(item.itemId, item);
        }
    }
    public ItemData GetItem(string itemId)
    {
        if (items.TryGetValue(itemId, out var item))
            return item.GetItemData();

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
        var Listtems = TestJson ? testItems : baseItems;
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
            var HeroDataDTO = new HeroDataDTO();
            foreach (var item in Listtems)
            {
                var itemData = item.GetItemData();
                var heroData = itemData as HeroData;
                HeroDataDTO.inventoryItems.Add(itemData);
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