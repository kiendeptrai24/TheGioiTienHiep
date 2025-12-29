using System;

[Serializable]
public class StatsRealmData : ItemData
{
    //Cultivation Realm
    public int maxHealth;
    public int maxMana;
    public int maxSpirit;

    //Offensive Stats
    public int critRate;
    public int critDamage;

    //Defensive Stats
    public int evasion;
    public int armorPenetration;

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