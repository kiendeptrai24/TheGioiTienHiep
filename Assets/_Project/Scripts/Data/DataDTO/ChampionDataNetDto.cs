using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct ChampionDataNetDto : INetworkSerializable
{
    public FixedString64Bytes instanceId;
    public FixedString64Bytes name;
    public bool isCharacter;
    public float manaPersent;
    public float healthPersent;

    public FixedString64Bytes raceId;
    public FixedString64Bytes essenceId;
    public FixedString64Bytes realmId;

    public int physicalDamagePoint;
    public int magicalDamagePoint;
    public int spiritDamagePoint;

    public int physicalDefensePoint;
    public int magicalDefensePoint;
    public int spiritDefensePoint;

    public int healthPoint;
    public int manaPoint;
    public int spiritPoint;

    public int moveSpeedPoint;
    public int spiritRangePoint;

    public Vector2Int championIndex;

    // Mỗi FixedList512Bytes chứa tối đa 7 items × 64 bytes = 448 bytes
    // 6 list × 512 bytes = 3072 bytes tổng — nằm trong giới hạn an toàn
    public FixedList512Bytes<FixedString64Bytes> equipmentIds;
    public FixedList512Bytes<FixedString64Bytes> equipmentIds1;
    public FixedList512Bytes<FixedString64Bytes> skillIds;
    public FixedList512Bytes<FixedString64Bytes> skillIds1;
    public FixedList512Bytes<FixedString64Bytes> techniqueIds;
    public FixedList512Bytes<FixedString64Bytes> techniqueIds1;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref instanceId);
        serializer.SerializeValue(ref name);

        serializer.SerializeValue(ref isCharacter);
        serializer.SerializeValue(ref manaPersent);
        serializer.SerializeValue(ref healthPersent);

        serializer.SerializeValue(ref raceId);
        serializer.SerializeValue(ref essenceId);
        serializer.SerializeValue(ref realmId);

        serializer.SerializeValue(ref physicalDamagePoint);
        serializer.SerializeValue(ref magicalDamagePoint);
        serializer.SerializeValue(ref spiritDamagePoint);

        serializer.SerializeValue(ref physicalDefensePoint);
        serializer.SerializeValue(ref magicalDefensePoint);
        serializer.SerializeValue(ref spiritDefensePoint);

        serializer.SerializeValue(ref healthPoint);
        serializer.SerializeValue(ref manaPoint);
        serializer.SerializeValue(ref spiritPoint);

        serializer.SerializeValue(ref moveSpeedPoint);
        serializer.SerializeValue(ref spiritRangePoint);

        serializer.SerializeValue(ref championIndex);

        SerializeList(serializer, ref equipmentIds);
        SerializeList(serializer, ref equipmentIds1);
        SerializeList(serializer, ref skillIds);
        SerializeList(serializer, ref skillIds1);
        SerializeList(serializer, ref techniqueIds);
        SerializeList(serializer, ref techniqueIds1);
    }

    private static void SerializeList<T>(
        BufferSerializer<T> serializer,
        ref FixedList512Bytes<FixedString64Bytes> list)
        where T : IReaderWriter
    {
        int count = list.Length;
        serializer.SerializeValue(ref count);

        if (serializer.IsReader)
        {
            list.Clear();
            for (int i = 0; i < count; i++)
            {
                FixedString64Bytes value = default;
                serializer.SerializeValue(ref value);
                list.Add(value);
            }
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                FixedString64Bytes value = list[i];
                serializer.SerializeValue(ref value);
            }
        }
    }
}