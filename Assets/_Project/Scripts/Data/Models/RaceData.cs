using System;
using Newtonsoft.Json;

[Serializable]
public class RaceData : ItemData
{
    //Race type
    [JsonIgnore]
    public RaceType raceType;

    //Resources (multipliers or % as you like)
    [JsonIgnore]
    public float healthPoint;
    [JsonIgnore]
    public float manaPoint;
    [JsonIgnore]
    public float spiritPoint;
    [JsonIgnore]
    public float physicalDamagePoint;
    [JsonIgnore]
    public float magicalDamagePoint;
    [JsonIgnore]
    public float spiritDamagePoint;
    [JsonIgnore]
    public float physicalDefensePoint;
    [JsonIgnore]
    public float magicalDefensePoint;
    [JsonIgnore]
    public float spiritDefensePoint;

    //Speed / Range
    [JsonIgnore]
    public float spiritRangePoint;
    [JsonIgnore]
    public float movementSpeedPoint;
}