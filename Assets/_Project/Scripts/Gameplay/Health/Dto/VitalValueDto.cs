using Unity.Netcode;

public struct VitalValueNetDto : INetworkSerializable
{
    public VitalType type;
    public int max;
    public int current;

    public VitalValueNetDto(VitalType type, int max, int current)
    {
        this.type = type;
        this.max = max;
        this.current = current;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer)
        where T : IReaderWriter
    {
        serializer.SerializeValue(ref type);
        serializer.SerializeValue(ref max);
        serializer.SerializeValue(ref current);
    }
}