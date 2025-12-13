using System;

[Serializable]
public class StatsRaceData
{
    //Race type
    public RaceType raceType;

    //Resources (multipliers or % as you like)
    public float health;
    public float mana;
    public float spirit;
    //Offensive Stats
    public float physicalDamage;
    public float magicalDamage;
    public float spiritDamage;

    //Defensive Stats
    public float physicalDefense;
    public float magicalDefense;
    public float spiritDefense;

    //Speed / Range
    public float spiritRange;
    public float movementSpeed;
}