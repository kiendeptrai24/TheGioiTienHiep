using Unity.Netcode;

public struct BattleEventDTO : INetworkSerializable
{
    public float t;
    public BattleEventType type;
    public int ownerUid;
    public int attackerUid;
    public int targetUid;

    public int damage;
    public bool isCrit;
    public int targetHpAfter;

    public string skillId;

    public short fromX, fromY, toX, toY; // chỉ dùng khi Move

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref t);
        serializer.SerializeValue(ref type);
        serializer.SerializeValue(ref attackerUid);
        serializer.SerializeValue(ref targetUid);
        serializer.SerializeValue(ref damage);
        serializer.SerializeValue(ref isCrit);
        serializer.SerializeValue(ref targetHpAfter);
        serializer.SerializeValue(ref skillId);
        serializer.SerializeValue(ref fromX);
        serializer.SerializeValue(ref fromY);
        serializer.SerializeValue(ref toX);
        serializer.SerializeValue(ref toY);
    }
}
