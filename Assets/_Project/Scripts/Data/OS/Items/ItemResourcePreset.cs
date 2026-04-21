using UnityEngine;

[CreateAssetMenu(fileName = "NewMaterialPreset", menuName = "RPG/Items/Resourse Preset")]
public class ItemResourcePreset : ItemPreset
{
    public RealmType cultivationStage;
    public ResourceSourceType resourceSourceType;
    public int yieldPerHarvest;
    public float miningTime;
    public float currentMiningProgress;
    public int maxStorage;
    public int currentAmount;
    public int level;
    public override ItemData GetItemData()
    {
        ItemResourseData data = new ItemResourseData();
        data.instanceId = instanceId;
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
        data.resourceSourceType = resourceSourceType;
        data.yieldPerHarvest = yieldPerHarvest;
        data.miningTime = miningTime;
        data.currentMiningProgress = currentMiningProgress;
        data.maxStorage = maxStorage;
        data.currentAmount = currentAmount;
        data.level = level;
        return data;
    }

}