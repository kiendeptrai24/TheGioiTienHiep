using System;

[Serializable]
public class StatsRaceData : ItemData
{
    //Race type
    public RaceType raceType;

    //Resources (multipliers or % as you like)
    public float maxHealth;
    public float maxMana;
    public float maxSpirit;
    //Speed / Range
    public float spiritRange;
    public float movementSpeed;
}