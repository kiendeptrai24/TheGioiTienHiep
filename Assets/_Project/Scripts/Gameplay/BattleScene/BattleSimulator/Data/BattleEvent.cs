using System;
using UnityEngine;

[Serializable]
public class BattleEvent
{
    public float time;
    public TeamId team;
    public BattleEventType type;
    public string ownerUid;
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
    public TeamId targetTeam;
    public string targetUid;
    public string attackerUid;
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
    public TeamId targetTeam;
    public string targetUid;
    public string attackerUid;
}

[Serializable]
public class BattleEventInit : BattleEvent
{
    public Vector2Int cell;
    public int maxHp;
    public int curtHp;
}
