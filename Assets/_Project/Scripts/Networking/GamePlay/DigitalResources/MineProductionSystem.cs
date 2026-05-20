using UnityEngine;

public class MineProductionSystem
{
    private readonly MineNetworkState networkState;

    private double lastProduceTime;
    private int yieldPerHarvest;
    private float miningTime;
    private int maxStorage;

    public MineProductionSystem(
        SpiritStoneMineData data,
        MineNetworkState networkState)
    {
        yieldPerHarvest = data.yieldPerHarvest;
        miningTime = data.miningTime;
        maxStorage = data.maxStorage;
        this.networkState = networkState;
    }

    public void ResetTime(double now)
    {
        lastProduceTime = now;
    }

    public bool Tick(
        double now,
        ResourceStorage storage)
    {
        if (storage == null)
            return false;

        if (now - lastProduceTime <
            miningTime)
            return false;

        int ticks = Mathf.FloorToInt(
            (float)(now - lastProduceTime));

        lastProduceTime += ticks;

        Produce(ticks, storage);
        return true;
    }

    private void Produce(
        int times,
        ResourceStorage storage)
    {
        int amount =
            yieldPerHarvest * times;

        if (networkState.currentAmount + amount >
            maxStorage)
        {
            amount =
                maxStorage -
                networkState.currentAmount;
        }

        networkState.currentAmount += amount;
        networkState.currentMiningProgress += times;

        storage.PlusCost((ulong)amount);
    }
}