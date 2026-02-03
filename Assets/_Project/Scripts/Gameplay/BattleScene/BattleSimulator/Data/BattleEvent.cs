using System;
using UnityEngine;

[Serializable]
public class BattleEvent
{
    public float t;
    public BattleEventType type;
    public int ownerUid;
}

[Serializable]
public class BattleEventMove : BattleEvent
{
    public Vector2Int from;
    public Vector2Int to;
}

[Serializable]
public class BattleEventAttack : BattleEvent
{
    public int targetUid;
    public int attackerUid;
    public int damage;
    public bool isCrit;
    public int targetHpAfter;
}

[Serializable]
public class BattleEventSkill : BattleEventAttack
{
    public string skillId;
}

[Serializable]
public class BattleEventDealth : BattleEvent
{
    public int targetUid;
    public int attackerUid;
}

