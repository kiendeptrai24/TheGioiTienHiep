using UnityEngine;

[CreateAssetMenu(fileName = "NewMaterialPreset", menuName = "RPG/Items/Resourse Preset")]
public class ItemResourcePreset : ItemPreset
{
    public RealmType cultivationStage;
    public ResourceType resourceType;

    public override ItemData GetItemData()
    {
        ItemResourseData data = new ItemResourseData();
        data.itemId = itemId;
        data.itemName = itemName;
        data.itemType = itemType;
        data.itemIcon = itemIcon;
        data.itemIconPath = itemIconPath;
        data.itemPrice = itemPrice;
        data.itemDescription = itemDescription;
        data.currentstack = currentstack;
        data.canStack = canStack;
        data.qualityType = qualityType;
        data.realmType = cultivationStage;
        data.resourceType = resourceType;
        return data;
    }

}