
using System;
using Newtonsoft.Json;

[Serializable]
public class StatsCultivationPathData : ItemData
{
    //Main cultivation type
    [JsonIgnore]
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
    public int maxHealth;
    [JsonIgnore]
    public int maxMana;
    [JsonIgnore]
    public int maxSpirit;

    //Speed / Range (per point)
    [JsonIgnore]
    public int movementSpeed;
    [JsonIgnore]
    public int spiritRange;

}