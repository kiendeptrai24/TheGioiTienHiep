using System;

[Serializable]
public class StatsRealmData : ItemData
{
    //Cultivation Realm
    public int health;
    public int mana;
    public int spirit;

    //Offensive Stats
    public int critChance;
    public int critPower;

    //Defensive Stats
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