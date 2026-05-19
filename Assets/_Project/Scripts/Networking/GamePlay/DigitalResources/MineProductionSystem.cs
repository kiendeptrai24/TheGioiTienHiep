using UnityEngine;

public class MineProductionSystem
{
    private readonly SpiritStoneMineData data;
    private readonly MineNetworkState networkState;

    private double lastProduceTime;

    public MineProductionSystem(
        SpiritStoneMineData data,
        MineNetworkState networkState)
    {
        this.data = data;
        this.networkState = networkState;
    }

    public void ResetTime(double now)
    {
        lastProduceTime = now;
    }

    public void Tick(
        double now,
        ResourceStorage storage)
    {
        if (storage == null)
            return;

        if (now - lastProduceTime <
            data.miningTime)
            return;

        int ticks = Mathf.FloorToInt(
            (float)(now - lastProduceTime));

        lastProduceTime += ticks;

        Produce(ticks, storage);
    }

    private void Produce(
        int times,
        ResourceStorage storage)
    {
        int amount =
            data.yieldPerHarvest * times;

        if (data.currentAmount + amount >
            data.maxStorage)
        {
            amount =
                data.maxStorage -
                data.currentAmount;
        }

        data.currentAmount += amount;

        networkState.currentAmount =
            data.currentAmount;

        storage.PlusCost((ulong)amount);
    }
}