using System;
using Unity.Netcode;

public class ResourceStorage : TGTHNetworkBehaviour
{

    public NetworkVariable<ulong> SpiritStone = new(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    public event Action<ulong> OnSpiritStoneChanged;
    private PlayerInventory inventory;
    protected override void Awake()
    {
        base.Awake();
        inventory = GetComponent<PlayerInventory>();
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        SpiritStone.OnValueChanged += HandleCoinsChanged;
    }
    private void OnDestroy()
    {
        SpiritStone.OnValueChanged -= HandleCoinsChanged;
    }
    public void InitSpiritStone(ulong coins)
    {
        if (!IsServer) return;
        SpiritStone.Value = coins;
    }
    private void HandleCoinsChanged(ulong oldValue, ulong newValue)
    {
        OnSpiritStoneChanged?.Invoke(newValue);
    }

    public bool HasEnough(ulong amount)
    {
        if (!IsServer) return false;
        return SpiritStone.Value >= amount;
    }

    // ===== SERVER ONLY =====
    public void PlusCost(ulong amount)
    {
        if (!IsServer) return;
        SpiritStone.Value += amount;
    }

    public void MinusCost(ulong amount)
    {
        if (!IsServer) return;
        SpiritStone.Value -= amount;
    }
    public void SetPlayerResource(PlayerResource playerResource)
    {
        if (!IsServer) return;
        InitSpiritStone((ulong)playerResource.linhThach);
        inventory.SetItems(playerResource.itemAmounts);
    }
    // ===== OFFLINE COINS =====
    public void AddOfflineCoins(ulong amount)
    {
        if (!IsServer) return;
        SpiritStone.Value += amount;
    }
}
