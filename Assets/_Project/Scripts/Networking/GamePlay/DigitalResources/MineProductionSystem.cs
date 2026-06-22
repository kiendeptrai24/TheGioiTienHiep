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
        out int producedAmount)
    {
        producedAmount = 0;

        if (now - lastProduceTime <
            miningTime)
            return false;

        int ticks = Mathf.FloorToInt(
            (float)(now - lastProduceTime));

        lastProduceTime += ticks;

        producedAmount = Produce(ticks);
        return true;
    }

    private int Produce(
        int times)
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
        return amount;
    }
}