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
    public int targetHpAfter;
    public string skillId0;
    public string skillId1;
    public string skillId2;
    public string skillId3;
    public string skillId4;
    public Vector2Int cell;
    public float castTime;

    public short fromX, fromY, toX, toY; // chỉ dùng khi Move
    public int moveSpeed;

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
        serializer.SerializeValue(ref targetHpAfter);
        serializer.SerializeValue(ref skillId0);
        serializer.SerializeValue(ref skillId1);
        serializer.SerializeValue(ref skillId2);
        serializer.SerializeValue(ref skillId3);
        serializer.SerializeValue(ref skillId4);
        serializer.SerializeValue(ref fromX);
        serializer.SerializeValue(ref fromY);
        serializer.SerializeValue(ref toX);
        serializer.SerializeValue(ref toY);
        serializer.SerializeValue(ref cell);
        serializer.SerializeValue(ref castTime);
        serializer.SerializeValue(ref moveSpeed);
    }
}
