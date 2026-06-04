using System;
using System.Collections.Generic;
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
    public TeamId targetTeam;
    public string targetUid;
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
    public float castTime;
}

[Serializable]
public class BattleEventSkill : BattleEventAttack
{
    public int healthCost;
    public int manaCost;
    public int spiritCost;
    public string skillId;
}

[Serializable]
public class BattleEventDealth : BattleEvent
{
    public TeamId targetTeam;
    public TeamId attackerTeam;
    public string targetUid;
    public string attackerUid;
}

[Serializable]
public class BattleEventInit : BattleEvent
{
    public Vector2Int cell;
    public int maxHp;
    public int curtHp;
    public int maxMana;
    public int curMana;
    public int maxSpirit;
    public int curSpirit;
    public int moveSpeed;
    public List<string> skillIds = new();
}

[Serializable]
public class BattleEventEnd : BattleEvent
{
    public bool heroIsPlayerObject;
    public int maxHealthHero;
    public int maxManaHero;
    public int maxSpiritHero;
    public int maxSpiritEnemy;
    public int curHealthHero;
    public int curManaHero;

    public bool enemyIsPlayerObject;
    public int maxHealthEnemy;
    public int maxManaEnemy;
    public int curSpiritHero;
    public int curHealthEnemy;
    public int curManaEnemy;
    public int curSpiritEnemy;
}
