using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct BattleEventDTO : INetworkSerializable
{
    public float t;
    public BattleEventType type;
    public TeamId team;
    public TeamId targetTeam;
    public TeamId attackerTeam;
    public string ownerUid;
    public string attackerUid;
    public string targetUid;

    public int damage;
    public bool isCrit;
    public int maxHp;
    public int curHp;
    public int maxMana;
    public int curMana;
    public int maxSpirit;
    public int curSpirit;
    public int targetHpAfter;
    public int costHealth;
    public int costMana;
    public int costSpirit;
    public string skillId0;
    public string skillId1;
    public string skillId2;
    public string skillId3;
    public string skillId4;
    public Vector2Int cell;
    public float castTime;

    public short fromX, fromY, toX, toY; // chỉ dùng khi Move
    public int moveSpeed;
    public int maxHealthPlayer1, maxManaPlayer1, maxSpiritPlayer1, maxHealthPlayer2, maxManaPlayer2, maxSpiritPlayer2;
    public int curHealthPlayer1, curManaPlayer1, curSpiritPlayer1, curHealthPlayer2, curManaPlayer2, curSpiritPlayer2;
    public bool heroIsPlayerObject;
    public bool enemyIsPlayerObject;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref t);
        serializer.SerializeValue(ref team);
        serializer.SerializeValue(ref targetTeam);
        serializer.SerializeValue(ref attackerTeam);
        serializer.SerializeValue(ref type);
        serializer.SerializeValue(ref ownerUid);
        serializer.SerializeValue(ref attackerUid);
        serializer.SerializeValue(ref targetUid);
        serializer.SerializeValue(ref damage);
        serializer.SerializeValue(ref isCrit);
        serializer.SerializeValue(ref maxHp);
        serializer.SerializeValue(ref curHp);
        serializer.SerializeValue(ref maxMana);
        serializer.SerializeValue(ref curMana);
        serializer.SerializeValue(ref maxSpirit);
        serializer.SerializeValue(ref curSpirit);
        serializer.SerializeValue(ref targetHpAfter);
        serializer.SerializeValue(ref skillId0);
        serializer.SerializeValue(ref skillId1);
        serializer.SerializeValue(ref skillId2);
        serializer.SerializeValue(ref skillId3);
        serializer.SerializeValue(ref skillId4);
        serializer.SerializeValue(ref costHealth);
        serializer.SerializeValue(ref costMana);
        serializer.SerializeValue(ref costSpirit);
        serializer.SerializeValue(ref fromX);
        serializer.SerializeValue(ref fromY);
        serializer.SerializeValue(ref toX);
        serializer.SerializeValue(ref toY);
        serializer.SerializeValue(ref cell);
        serializer.SerializeValue(ref castTime);
        serializer.SerializeValue(ref moveSpeed);
        serializer.SerializeValue(ref maxHealthPlayer1);
        serializer.SerializeValue(ref maxManaPlayer1);
        serializer.SerializeValue(ref maxSpiritPlayer1);
        serializer.SerializeValue(ref maxHealthPlayer2);
        serializer.SerializeValue(ref maxManaPlayer2);
        serializer.SerializeValue(ref maxSpiritPlayer2);
        serializer.SerializeValue(ref curHealthPlayer1);
        serializer.SerializeValue(ref curManaPlayer1);
        serializer.SerializeValue(ref curSpiritPlayer1);
        serializer.SerializeValue(ref curHealthPlayer2);
        serializer.SerializeValue(ref curManaPlayer2);
        serializer.SerializeValue(ref curSpiritPlayer2);
        serializer.SerializeValue(ref heroIsPlayerObject);
        serializer.SerializeValue(ref enemyIsPlayerObject);
    }
}
