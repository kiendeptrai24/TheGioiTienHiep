
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
    public int healthPoint;
    public int manaPoint;
    public int spiritPoint;
    public int physicalDamagePoint;
    public int magicalDamagePoint;
    public int spiritDamagePoint;
    public int physicalDefensePoint;
    public int magicalDefensePoint;
    public int spiritDefensePoint;


    //Speed / Range (per point)
    public int movementSpeedPoint;
    public int spiritRangePoint;
    override public ItemData Clone()
    {
        return (EssenceData)this.MemberwiseClone();
    }
}