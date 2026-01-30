using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public enum TeamId : byte { Heroes = 0, Enemies = 1 }

[Serializable]
public struct UnitSnapshot
{
    public int uid;          // index trong list
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
}

public enum BattleEventType : byte { Attack = 0, Death = 1, End = 2 }

[Serializable]
public struct BattleEvent
{
    public float t;
    public BattleEventType type;
    public int attackerUid;
    public int targetUid;
    public int damage;
    public bool isCrit;
    public int targetHpAfter;
}
public struct BattleEventDTO : INetworkSerializable
{
    public float t;
    public BattleEventType type;
    public int attackerUid;
    public int targetUid;
    public int damage;
    public bool isCrit;
    public int targetHpAfter;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer)
        where T : IReaderWriter
    {
        serializer.SerializeValue(ref t);
        serializer.SerializeValue(ref type);
        serializer.SerializeValue(ref attackerUid);
        serializer.SerializeValue(ref targetUid);
        serializer.SerializeValue(ref damage);
        serializer.SerializeValue(ref isCrit);
        serializer.SerializeValue(ref targetHpAfter);
    }
}