using System;
using Newtonsoft.Json;

[Serializable]
public class RaceData : ItemData
{
    public RaceType raceType;

    //Resources (multipliers or % as you like)
    public float healthPoint;
    public float manaPoint;
    public float spiritPoint;
    public float physicalDamagePoint;
    public float magicalDamagePoint;
    public float spiritDamagePoint;
    public float physicalDefensePoint;
    public float magicalDefensePoint;
    public float spiritDefensePoint;

    //Speed / Range
    public float spiritRangePoint;
    public float movementSpeedPoint;
    override public ItemData Clone()
    {
        return (RaceData)this.MemberwiseClone();
    }
}