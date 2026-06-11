using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.Netcode;
using UnityEngine;

public class PlayerVitals : TGTHNetworkBehaviour
{
    private readonly PlayerVitalData vitals = new();

    [SerializeField] private NetworkVariable<int> MaxHealth = new(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField] private NetworkVariable<int> CurrentHealth = new(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [SerializeField] private NetworkVariable<int> MaxMana = new(1, NetworkVariableReadPermission.Owner, NetworkVariableWritePermission.Server);
    [SerializeField] private NetworkVariable<int> CurrentMana = new(1, NetworkVariableReadPermission.Owner, NetworkVariableWritePermission.Server);

    [SerializeField] private NetworkVariable<int> MaxSpirit = new(1, NetworkVariableReadPermission.Owner, NetworkVariableWritePermission.Server);
    [SerializeField] private NetworkVariable<int> CurrentSpirit = new(1, NetworkVariableReadPermission.Owner, NetworkVariableWritePermission.Server);
    private StatsData statsData;
    public event Action<VitalType, int, int> OnVitalChanged;
    private PlayerBattleRoster roster;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        roster = GetComponentInParent<PlayerBattleRoster>();
        SubscribeVitalEvents();
        if (IsOwner)
        {
            UpgradeSystemManager.Instance.OnRealmUpgrade += OnRealmUpgrade;

            statsData = GetComponent<StatsData>();
            statsData.OnStatReady += OnStatReady;
            statsData.SetUpItem(InventoryCenterManager.Instance.playerCham);
        }
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        UnsubscribeVitalEvents();
    }
    private void SubscribeVitalEvents()
    {
        MaxHealth.OnValueChanged += OnHealthChanged;
        CurrentHealth.OnValueChanged += OnHealthChanged;

        MaxMana.OnValueChanged += OnManaChanged;
        CurrentMana.OnValueChanged += OnManaChanged;

        MaxSpirit.OnValueChanged += OnSpiritChanged;
        CurrentSpirit.OnValueChanged += OnSpiritChanged;
    }
    private void UnsubscribeVitalEvents()
    {
        MaxHealth.OnValueChanged -= OnHealthChanged;
        CurrentHealth.OnValueChanged -= OnHealthChanged;

        MaxMana.OnValueChanged -= OnManaChanged;
        CurrentMana.OnValueChanged -= OnManaChanged;

        MaxSpirit.OnValueChanged -= OnSpiritChanged;
        CurrentSpirit.OnValueChanged -= OnSpiritChanged;
    }
    private void OnRealmUpgrade(bool result)
    {
        if (result)
        {
            if (!result) return;

            statsData.SetUpItem(InventoryCenterManager.Instance.playerCham);

            SetVitalServerRpc(
                new VitalValueNetDto(VitalType.Health, statsData.Health, statsData.Health),
                new VitalValueNetDto(VitalType.Mana, statsData.Mana, statsData.Mana),
                new VitalValueNetDto(VitalType.Spirit, statsData.Spirit, statsData.Spirit)
            );
        }
    }
    #region Callback Sync

    private void OnSpiritChanged(int maxValue, int curValue) => NotiVitalChanged(VitalType.Spirit, MaxSpirit.Value, CurrentSpirit.Value);

    private void OnManaChanged(int maxValue, int curValue) => NotiVitalChanged(VitalType.Mana, MaxMana.Value, CurrentMana.Value);

    private void OnHealthChanged(int maxValue, int curValue) => NotiVitalChanged(VitalType.Health, MaxHealth.Value, CurrentHealth.Value);

    #endregion
    public (int max, int current) GetVital(VitalType type)
    {
        switch (type)
        {
            case VitalType.Health:
                return (MaxHealth.Value, CurrentHealth.Value);
            case VitalType.Mana:
                return (MaxMana.Value, CurrentMana.Value);
            case VitalType.Spirit:
                return (MaxSpirit.Value, CurrentSpirit.Value);
        }
        return (1, 1);
    }
    public int GetCurrent(VitalType type)
    {
        switch (type)
        {
            case VitalType.Health:
                return CurrentHealth.Value;
            case VitalType.Mana:
                return CurrentMana.Value;
            case VitalType.Spirit:
                return CurrentSpirit.Value;
        }
        return 1;
    }
    private void OnStatReady(StatsData data)
    {
        var profile = ProfileManager.Instance.GetProfile();

        SetVitalServerRpc(
            new VitalValueNetDto(VitalType.Health, data.Health, profile.currentHealth),
            new VitalValueNetDto(VitalType.Mana, data.Mana, profile.currentMana),
            new VitalValueNetDto(VitalType.Spirit, data.Spirit, profile.currentSpirit)
        );
    }
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    private void SetVitalServerRpc(
        VitalValueNetDto health,
        VitalValueNetDto mana,
        VitalValueNetDto spirit)
    {
        SetVital(health.type, health.max, health.current);
        SetVital(mana.type, mana.max, mana.current);
        SetVital(spirit.type, spirit.max, spirit.current);
    }
    public void SetViral(float healthPersent, float manaPersent, float spiritPersent)
    {
        if (!IsServer) return;
        SetVital(VitalType.Health, MaxHealth.Value, Mathf.RoundToInt(MaxHealth.Value * healthPersent));
        SetVital(VitalType.Mana, MaxMana.Value, Mathf.RoundToInt(MaxMana.Value * manaPersent));
        SetVital(VitalType.Spirit, MaxSpirit.Value, Mathf.RoundToInt(MaxSpirit.Value * spiritPersent));
    }
    public void ResetViral()
    {
        if (!IsServer) return;
        int curHealth = vitals.Get(VitalType.Health).Max;
        int maxHealth = vitals.Get(VitalType.Health).Max;
        int curMana = vitals.Get(VitalType.Mana).Max;
        int maxMana = vitals.Get(VitalType.Mana).Max;
        int curSpirit = vitals.Get(VitalType.Spirit).Max;
        int maxSpirit = vitals.Get(VitalType.Spirit).Max;

        SetVital(VitalType.Health, maxHealth, curHealth);
        SetVital(VitalType.Mana, maxMana, curMana);
        SetVital(VitalType.Spirit, maxSpirit, curSpirit);
    }
    public void SetVital(VitalType type, int max, int current)
    {
        if (!IsServer) return;

        var vital = vitals.Get(type);
        vital.Set(max, current);

        SyncToNetwork(type, vital);
    }

    public void Decrease(VitalType type, int amount)
    {
        if (!IsServer) return;

        var vital = vitals.Get(type);
        vital.Decrease(amount);

        SyncToNetwork(type, vital);
    }
    public void Increase(VitalType type, int amount)
    {
        if (!IsServer) return;

        var vital = vitals.Get(type);
        vital.Increase(amount);

        SyncToNetwork(type, vital);
    }

    private void SyncToNetwork(VitalType type, VitalValue vital)
    {
        switch (type)
        {
            case VitalType.Health:
                MaxHealth.Value = vital.Max;
                CurrentHealth.Value = vital.Current;
                break;

            case VitalType.Mana:
                MaxMana.Value = vital.Max;
                CurrentMana.Value = vital.Current;
                break;

            case VitalType.Spirit:
                MaxSpirit.Value = vital.Max;
                CurrentSpirit.Value = vital.Current;
                break;
        }
    }
    private void NotiVitalChanged(VitalType type, int maxValue, int curValue)
    {

        float persent = Mathf.Clamp01((float)curValue / maxValue);
        var inventory = InventoryCenterManager.Instance;
        if (IsOwner)
        {
            switch (type)
            {
                case VitalType.Health:
                    inventory.championData.healthPersent = persent;
                    break;
                case VitalType.Mana:
                    inventory.championData.manaPersent = persent;
                    break;
                case VitalType.Spirit:
                    inventory.championData.spiritPersent = persent;
                    break;
            }

        }
        roster.SetCharacterPersent(type, persent);
        OnVitalChanged?.Invoke(type, maxValue, curValue);
    }
}