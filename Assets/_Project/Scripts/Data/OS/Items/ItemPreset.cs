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
    public string itemIconPath;

    public string itemDescription;
    public int itemPrice = 100;
    public int currentstack;
    public bool canStack = false;
    public QualityType qualityType;

    private void OnValidate()
    {
#if UNITY_EDITOR
        string path = AssetDatabase.GetAssetPath(this);
        itemId = AssetDatabase.AssetPathToGUID(path);

        if (itemIcon != null)
        {
            string iconPath = AssetDatabase.GetAssetPath(itemIcon);

            int index = iconPath.IndexOf("Resources/");
            if (index >= 0)
            {
                iconPath = iconPath.Substring(index + "Resources/".Length);
                iconPath = System.IO.Path.ChangeExtension(iconPath, null);
            }

            itemIconPath = iconPath + "_" + itemIcon.name.Split('_')[^1];
        }
#endif
    }

    public virtual ItemData GetItemData()
    {
        ItemData data = new ItemData();

        data.itemId = itemId;
        data.itemName = itemName;
        data.itemType = itemType;

        // ❌ không serialize sprite
        // data.itemIcon = itemIcon;

        data.itemIconPath = itemIconPath;

        data.itemPrice = itemPrice;
        data.itemDescription = itemDescription;
        data.currentstack = currentstack;
        data.canStack = canStack;
        data.qualityType = qualityType;

        return data;
    }
}