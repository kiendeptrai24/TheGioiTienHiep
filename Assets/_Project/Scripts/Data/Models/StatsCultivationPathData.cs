
using System;

[Serializable]
public class StatsCultivationPathData : ItemData
{
    //Main cultivation type
    public EssenceType essenceType;

    //Counter cultivation type
    public EssenceType counterEssenceType;

    // 0.2 = giảm 20% phòng ngự của essence bị khắc chế
    //, 1
    public float counterPercentage;

    //Resources (per point)
    public int maxHealth;
    public int maxMana;
    public int maxSpirit;

    //Speed / Range (per point)
    public int movementSpeed;
    public int spiritRange;

}