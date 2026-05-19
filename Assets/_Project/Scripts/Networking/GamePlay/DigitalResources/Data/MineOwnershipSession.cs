public class MineOwnershipSession
{
    public string PlayerId;
    public string MineId;
    public long StartTime;
    public long EndTime;       // 0 = đang active
    public ulong YieldPerSecond;
    public long OfflineTime;   // 0 = player đang online
}