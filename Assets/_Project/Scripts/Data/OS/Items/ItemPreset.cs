#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(fileName = "NewMaterialPreset", menuName = "RPG/Items/Material Preset")]
public abstract class ItemPreset : ScriptableObject
{
    public string instanceId;
    public string itemId;
    public string itemName;
    public ItemType itemType;

    public Sprite itemIcon;
    public string itemIconPath;
    public string itemFilePath;

    public string itemDescription;
    public ulong itemPrice = 100;
    public int currentstack;
    public bool canStack = false;
    public QualityType qualityType;

    public virtual void OnValidate()
    {
#if UNITY_EDITOR
        string path = AssetDatabase.GetAssetPath(this);
        instanceId = AssetDatabase.AssetPathToGUID(path);
        itemId = System.Guid.NewGuid().ToString();

        if (itemIcon != null)
        {
            itemIconPath = itemIcon.name;
            itemFilePath = "";
        }
#endif
    }

    public virtual ItemData GetItemData()
    {
        ItemData data = new ItemData();
        data.instanceId = instanceId;
        data.itemId = itemId;
        data.itemName = itemName;
        data.itemType = itemType;

        data.itemIcon = itemIcon;
        data.itemFilePath = itemFilePath;
        data.itemIconPath = itemIconPath;

        data.itemPrice = itemPrice;
        data.itemDescription = itemDescription;
        data.currentstack = currentstack;
        data.canStack = canStack;
        data.qualityType = qualityType;

        return data;
    }
}