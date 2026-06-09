using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct ChampionDataNetDto : INetworkSerializable
{
    public FixedString64Bytes instanceId;

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

    public FixedList512Bytes<FixedString64Bytes> equipmentIds;
    public FixedList512Bytes<FixedString64Bytes> skillIds;
    public FixedList512Bytes<FixedString64Bytes> techniqueIds;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer)
        where T : IReaderWriter
    {
        serializer.SerializeValue(ref instanceId);

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

        SerializeFixedStringList(serializer, ref equipmentIds);
        SerializeFixedStringList(serializer, ref skillIds);
        SerializeFixedStringList(serializer, ref techniqueIds);
    }
    private static void SerializeFixedStringList<T>(
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