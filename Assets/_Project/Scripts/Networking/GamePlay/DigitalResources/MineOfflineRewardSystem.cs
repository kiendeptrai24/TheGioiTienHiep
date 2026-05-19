using UnityEngine;

public class MineOfflineRewardSystem
{
    private readonly SpiritStoneMineData data;

    public MineOfflineRewardSystem(SpiritStoneMineData data)
    {
        this.data = data;
    }

    public ulong GetReward(
        float start,
        float end)
    {
        double duration = end - start;

        float yieldPerSecond =
            data.yieldPerHarvest /
            data.miningTime;

        return (ulong)(
            yieldPerSecond *
            duration
        );
    }
    public void GetReward(ulong NetId, string playerId, float start, float end)
    {
    }
    
}