

using Newtonsoft.Json;
using UnityEngine;
[System.Serializable]
public class SpiritStoneMineData : ItemResourseData
{
    public int yieldPerHarvest; // số lượng coins thu hoach được trong 1 lần

    public float miningTime; // thời gian để thu hoạch
    [JsonIgnore]
    public float currentMiningProgress; // thời gian đã thu hoạch
    public int maxStorage; // max coins có thể thu hoạch
    [JsonIgnore]
    public int currentAmount; // số coins đã bị thu hoạch
    public int level; // cấp của mỏ

}