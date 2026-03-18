using System;
using Newtonsoft.Json;

[Serializable]
public class StatsRaceData : ItemData
{
    //Race type
    [JsonIgnore]
    public RaceType raceType;

    //Resources (multipliers or % as you like)
    [JsonIgnore]
    public float maxHealth;
    [JsonIgnore]
    public float maxMana;
    [JsonIgnore]
    public float maxSpirit;
    //Speed / Range
    [JsonIgnore]
    public float spiritRange;
    [JsonIgnore]
    public float movementSpeed;
}