using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.Netcode;
using UnityEngine;

public class PlayerVitals : TGTHNetworkBehaviour
{
    private readonly PlayerVitalData vitals = new();

    [SerializeField] private NetworkVariable<int> MaxHealth = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField] private NetworkVariable<int> CurrentHealth = new(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [SerializeField] private NetworkVariable<int> MaxMana = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField] private NetworkVariable<int> CurrentMana = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [SerializeField] private NetworkVariable<int> MaxSpirit = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField] private NetworkVariable<int> CurrentSpirit = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public event Action<VitalType, int, int> OnVitalChanged;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            MaxHealth.OnValueChanged += OnMaxHealthChanged;
            CurrentHealth.OnValueChanged += OnCurrentHealthChanged;
            MaxMana.OnValueChanged += OnMaxManaChanged;
            CurrentMana.OnValueChanged += OnCurrentManaChanged;
            MaxSpirit.OnValueChanged += OnMaxSpiritChanged;
            CurrentSpirit.OnValueChanged += OnCurrentSpiritChanged;

            StatsData statsData = GetComponent<StatsData>();
            statsData.OnStatReady += OnStatReady;
            statsData.SetUpItem(InventoryCenterManager.Instance.playerCham);
        }
    }
    #region Callback Sync

    private void OnCurrentSpiritChanged(int maxValue, int curValue) => NotiVitalChanged(VitalType.Spirit, MaxSpirit.Value, CurrentSpirit.Value);

    private void OnMaxSpiritChanged(int maxValue, int curValue) => NotiVitalChanged(VitalType.Spirit, MaxSpirit.Value, CurrentSpirit.Value);

    private void OnCurrentManaChanged(int maxValue, int curValue) => NotiVitalChanged(VitalType.Mana, MaxMana.Value, CurrentMana.Value);

    private void OnMaxManaChanged(int maxValue, int curValue) => NotiVitalChanged(VitalType.Mana, MaxMana.Value, CurrentMana.Value);

    private void OnCurrentHealthChanged(int maxValue, int curValue) => NotiVitalChanged(VitalType.Health, MaxHealth.Value, CurrentHealth.Value);

    private void OnMaxHealthChanged(int maxValue, int curValue) => NotiVitalChanged(VitalType.Health, MaxHealth.Value, CurrentHealth.Value);

    #endregion
    public (int max, int current) GetVital(VitalType type)
    {
        var vital = vitals.Get(type);

        return (vital.Max, vital.Current);
    }
    public int GetCurrent(VitalType type)
    {
        var vital = vitals.Get(type);

        return vital.Current;
    }
    private void OnStatReady(StatsData data)
    {
        var profile = ProfileManager.Instance.GetProfile();

        var curHealth = profile.currentHealth;
        var curMana = profile.currentMana;
        var curSpirit = profile.currentSpirit;
        var dataDto = new List<VitalValueDto>();

        dataDto.Add(new VitalValueDto(VitalType.Health.ToString(), data.Health, curHealth));
        dataDto.Add(new VitalValueDto(VitalType.Mana.ToString(), data.Mana, curMana));
        dataDto.Add(new VitalValueDto(VitalType.Spirit.ToString(), data.Spirit, curSpirit));

        string payload = JsonConvert.SerializeObject(dataDto);
        SetVitalServerRpc(payload);
    }
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    private void SetVitalServerRpc(string data)
    {
        var dataDto = JsonConvert.DeserializeObject<List<VitalValueDto>>(data);
        foreach (var dto in dataDto)
        {
            SetVital(Enum.Parse<VitalType>(dto.type), dto.max, dto.current);
        }
    }
    public void SetViral(float healthPersent, float manaPersent, float spiritPersent)
    {
        if (!IsServer) return;
        CurrentHealth.Value = Mathf.RoundToInt(MaxHealth.Value * healthPersent);
        CurrentMana.Value = Mathf.RoundToInt(MaxMana.Value * manaPersent);
        CurrentSpirit.Value = Mathf.RoundToInt(MaxSpirit.Value * spiritPersent);
    }
    public void ResetViral()
    {
        if (!IsServer) return;
        CurrentHealth.Value = MaxHealth.Value;
        CurrentMana.Value = MaxMana.Value;
        CurrentSpirit.Value = MaxSpirit.Value;
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
        InventoryCenterManager.Instance.NotifyListItemDatasChampionChanged();
        OnVitalChanged?.Invoke(type, maxValue, curValue);
    }
}