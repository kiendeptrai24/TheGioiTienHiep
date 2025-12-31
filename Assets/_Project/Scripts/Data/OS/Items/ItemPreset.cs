#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(fileName = "NewMaterialPreset", menuName = "RPG/Items/Material Preset")]
public abstract class ItemPreset : ScriptableObject
{
    public string itemId;
    public string itemName;
    public ItemType itemType;
    public Sprite itemIcon;
    public string itemDescription;
    public int currentstack;
    public QualityType qualityType;
    private void OnValidate()
    {
#if UNITY_EDITOR
        string path = AssetDatabase.GetAssetPath(this);
        itemId = AssetDatabase.AssetPathToGUID(path);
#endif
    }
    public virtual ItemData GetItemData()
    {
        ItemData data = new ItemData();
        data.itemId = itemId;
        data.itemName = itemName;
        data.itemType = itemType;
        data.itemIcon = itemIcon;
        data.itemDescription = itemDescription;
        data.currentstack = currentstack;
        return data;
    }

}