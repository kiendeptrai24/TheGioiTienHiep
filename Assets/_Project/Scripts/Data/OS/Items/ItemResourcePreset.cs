#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(fileName = "NewMaterialPreset", menuName = "RPG/Items/Resourse Preset")]
public class ItemResourcePreset : ItemPreset
{
    public CultivationStage cultivationStage;
    public ResourceType resourceType;
    private void OnValidate()
    {
#if UNITY_EDITOR
        string path = AssetDatabase.GetAssetPath(this);
        itemId = AssetDatabase.AssetPathToGUID(path);
#endif
    }
    public override ItemData GetItemData()
    {
        ItemResourseData data = new ItemResourseData();
        data.itemId = itemId;
        data.itemName = itemName;
        data.itemType = itemType;
        data.itemIcon = itemIcon;
        data.itemPrice = itemPrice;
        data.itemDescription = itemDescription;
        data.currentstack = currentstack;
        data.canStack = canStack;
        data.qualityType = qualityType;
        data.cultivationStage = cultivationStage;
        data.resourceType = resourceType;
        return data;
    }

}