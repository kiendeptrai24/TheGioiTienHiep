
using System;

[Serializable]
public class StatsCultivationPathData
{
    //Main cultivation type
    public EssenceType essenceType;

    //Counter cultivation type
    public EssenceType counterEssenceType;

    // 0.2 = giảm 20% phòng ngự của essence bị khắc chế
    //, 1
    public float counterPercentage;

    //Resources (per point)
    public int health;
    public int mana;
    public int spirit;

    //Offensive Stats (per point)
    public int physicalDamage;
    public int magicalDamage;
    public int spiritDamage;

    //Defensive Stats (per point)
    public int physicalDefense;
    public int magicalDefense;
    public int spiritDefense;

    //Speed / Range (per point)
    public int movementSpeed;
    public int spiritRange;

}