

using Newtonsoft.Json;
using UnityEngine;
[System.Serializable]
public class ItemResourseData : ItemData
{
    [JsonIgnore]
    public ResourceType resourceType;
    [JsonIgnore]
    public Vector3 position;
    [JsonIgnore]
    public int yieldPerHarvest; // số lượng coins thu hoach được trong 1 lần

    [JsonIgnore]
    public float miningTime; // thời gian để thu hoạch
    [JsonIgnore]
    public float currentMiningProgress; // thời gian đã thu hoạch
    [JsonIgnore]
    public int maxStorage; // max coins có thể thu hoạch
    [JsonIgnore]
    public int currentAmount; // số coins đã bị thu hoạch
    [JsonIgnore]
    public int level; // cấp của mỏ

}