


using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerHealth : TGTHNetworkBehaviour
{
    [SerializeField]
    private NetworkVariable<int> MaxHealth =
        new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone,
         NetworkVariableWritePermission.Server);
    [SerializeField]
    private NetworkVariable<int> CurrentHealth =
        new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone,
         NetworkVariableWritePermission.Server);
    public event Action<int, int> OnHealthChanged;
    public int GetMaxHealth() => MaxHealth.Value;
    public int GetCurHealth() => CurrentHealth.Value;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            MaxHealth.OnValueChanged += OnMaxHealthChanged;
            CurrentHealth.OnValueChanged += OnCurrentHealthChanged;
            StatsData statsData = GetComponent<StatsData>();
            statsData.OnStatReady += OnStatReady;
            statsData.SetUpItem(InventoryCenterManager.Instance.playerCham);
        }
    }
    public void ResetHealth()
    {
        if (!IsServer) return;
        CurrentHealth.Value = MaxHealth.Value;
    }
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    private void SentHealthToServerRpc(int maxHealth, int currentHealth)
    {
        SetHealth(maxHealth, currentHealth);
    }
    private void OnCurrentHealthChanged(int previousValue, int newValue)
    {
        NotiHealthChanged(MaxHealth.Value, CurrentHealth.Value);
    }

    private void OnMaxHealthChanged(int previousValue, int newValue)
    {
        NotiHealthChanged(MaxHealth.Value, CurrentHealth.Value);
    }

    private void OnStatReady(StatsData data)
    {
        var curHealth = ProfileManager.Instance.GetProfile().currentHealth;
        SentHealthToServerRpc(data.Health, curHealth);
    }
    public void SetHealth(int maxHealth, int currentHealth)
    {
        if (!IsServer) return;
        MaxHealth.Value = maxHealth;
        CurrentHealth.Value = currentHealth;
        NotiHealthChanged(maxHealth, currentHealth);
    }
    public void SetCurrentHealth(int currentHealth)
    {
        if (!IsServer) return;
        if (MaxHealth.Value < currentHealth)
        {
            CurrentHealth.Value = MaxHealth.Value;
        }
        else
        {
            CurrentHealth.Value = currentHealth;
        }
    }
    public void IncreaseHealth(int amount)
    {
        if (!IsServer)
            return;
        if (amount <= 0)
            return;
        CurrentHealth.Value = Mathf.Min(MaxHealth.Value, CurrentHealth.Value + amount);
    }
    private void NotiHealthChanged(int maxHealth, int currentHealth)
    {
        float persent = Mathf.Clamp01((float)currentHealth / maxHealth);
        InventoryCenterManager.Instance.championData.healthPersent = persent;
        InventoryCenterManager.Instance.NotifyListItemDatasChampionChanged();
        OnHealthChanged?.Invoke(maxHealth, currentHealth);
    }
    public void DecreaseHealth(int damage)
    {
        if (!IsServer)
            return;
        if (damage <= 0)
            return;
        CurrentHealth.Value = Mathf.Max(0, CurrentHealth.Value - damage);
    }
}
