using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class ScriptableObjectLoader : Singleton<ScriptableObjectLoader>
{
    public bool TestJson = false;
    public List<ItemPreset> baseItems = new();
    public List<ItemPreset> testItems = new();
    private Dictionary<string, ItemPreset> items = new();
    protected override void Awake()
    {
        base.Awake();
        foreach (var item in baseItems)
        {
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
        List<ItemData> items = new List<ItemData>();
        var Listtems = TestJson ? testItems : baseItems;
        foreach (var item in Listtems)
        {
            var itemData = item.GetItemData();
            items.Add(itemData);

        }
        ItemJsonCreator.CreateItemJson(items);
    }
#endif

}