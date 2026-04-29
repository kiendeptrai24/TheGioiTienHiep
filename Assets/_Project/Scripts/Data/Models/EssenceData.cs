
using System;
using Newtonsoft.Json;

[Serializable]
public class EssenceData : ItemData
{
    //Main cultivation type
    public EssenceType essenceType;

    //Counter cultivation type
    [JsonIgnore]
    public EssenceType counterEssenceType;

    // 0.2 = giảm 20% phòng ngự của essence bị khắc chế
    //, 1
    [JsonIgnore]
    public float counterPercentage;

    //Resources (per point)
    [JsonIgnore]
    public int healthPoint;
    [JsonIgnore]
    public int manaPoint;
    [JsonIgnore]
    public int spiritPoint;
    [JsonIgnore]
    public int physicalDamagePoint;
    [JsonIgnore]
    public int magicalDamagePoint;
    [JsonIgnore]
    public int spiritDamagePoint;
    [JsonIgnore]
    public int physicalDefensePoint;
    [JsonIgnore]
    public int magicalDefensePoint;
    [JsonIgnore]
    public int spiritDefensePoint;


    //Speed / Range (per point)
    [JsonIgnore]
    public int movementSpeedPoint;
    [JsonIgnore]
    public int spiritRangePoint;

}