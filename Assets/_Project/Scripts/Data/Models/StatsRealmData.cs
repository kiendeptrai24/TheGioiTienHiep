using System;

[Serializable]
public class StatsRealmData 
{
    //Cultivation Realm
    public CultivationStage cultivationStage;
    //Resources
    public int health;
    public int mana;
    public int spirit;

    //Offensive Stats
    public int physicalDamage;
    public int magicalDamage;
    public int spiritDamage;
    public int critChance;
    public int critPower;

    //Defensive Stats
    public int physicalDefense;
    public int magicalDefense;
    public int spiritDefense;
    public int evasion;
    public int spiritPenetration;

    //Speed Stats
    public int movementSpeed;
    public int attackSpeed;
    public int castSpeed;

    //Progression Stats
    public int potential;
    public int skillPoints;
    public int combatPower;
    //Critical Stats
    public int spiritRange;
}