using System;
using Unity.Collections;

[Serializable]
public class MineOwnershipSegment
{
    public FixedString64Bytes OwnerId;
    public float StartTime;
    public float EndTime;
}