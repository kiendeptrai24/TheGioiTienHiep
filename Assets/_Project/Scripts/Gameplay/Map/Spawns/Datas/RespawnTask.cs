using System;
[Serializable]
public class RespawnTask : IComparable<RespawnTask>
{
    public long Id;
    public double SpawnTime;
    public Action OnRespawn;

    public int CompareTo(RespawnTask other)
    {
        int compare = SpawnTime.CompareTo(other.SpawnTime);

        if (compare == 0)
            compare = Id.CompareTo(other.Id);

        return compare;
    }
}