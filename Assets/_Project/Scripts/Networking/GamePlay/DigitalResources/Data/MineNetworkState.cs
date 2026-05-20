using System;
using Unity.Netcode;
[Serializable]
public class MineNetworkState
{
    public NetworkObject Owner;
    public string playerId;
    public int currentAmount;
    public float currentMiningProgress;
}