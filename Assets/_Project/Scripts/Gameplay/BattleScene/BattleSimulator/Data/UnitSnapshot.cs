using System;
using Unity.Netcode;
[Serializable]
public enum TeamId : byte { Heroes = 0, Enemies = 1 }

[Serializable]
public class UnitSnapshot
{
    public string uid;          // index trong list
    public TeamId team;
    public int hpMax;
    public int hp;

    public int physicalDmg;
    public int magicalDmg;
    public int spiritDmg;
    public float trueDmg;

    public float armorPen;
    public float spiritPen;

    public int physicalDef;
    public int magicalDef;
    public int spiritDef;

    public float critChance;   // 0..1
    public float critPower;    // vd 2.0 = 200%
    public float critReduction; // 0..1

    public float penReduction;   // 0..1
    public float trueReduction;  // 0..1
    public float dmgImmunity;    // 0..1

    public float lifeSteal;    // theo logic bạn đang dùng: attacker.LifeSteal * totalDmg
    public float reflect;      // defender.ReflectDamage * totalDmg

    public int attackSpeed;    // dùng để tính interval
    public float attackRange;
}

public enum BattleEventType : byte
{
    Start = 0,
    Move = 1,
    Attack = 2,
    Skill = 3,
    Death = 4,
    End = 5,
    Init = 6,
    Stuck = 99,

}


