using System;

[Serializable]
public class UpgradeState : IComparable<UpgradeState>
{
    public string playerId;
    public string upgradeId;
    public long startTime;
    public long endTime;
    public bool isCompleted;
    public bool result;
    public int CompareTo(UpgradeState other)
    {
        if (other == null) return 1;

        // So sánh theo thời gian kết thúc (endTime)
        int result = this.endTime.CompareTo(other.endTime);

        // Nếu trùng endTime, so sánh tiếp playerId để tránh bị SortedSet nuốt mất dữ liệu trùng mốc thời gian
        if (result == 0)
        {
            return string.Compare(this.playerId, other.playerId, StringComparison.Ordinal);
        }

        return result;
    }
}