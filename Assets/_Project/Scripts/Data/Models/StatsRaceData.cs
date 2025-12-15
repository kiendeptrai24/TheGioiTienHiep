using System;

[Serializable]
public class StatsRaceData : ItemData
{
    //Race type
    public RaceType raceType;

    //Resources (multipliers or % as you like)
    public float health;
    public float mana;
    public float spirit;
    //Speed / Range
    public float spiritRange;
    public float movementSpeed;
}