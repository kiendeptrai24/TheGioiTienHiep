using System;
using Unity.Netcode;
using UnityEngine;

public abstract class HealthController : TGTHNetworkBehaviour
{
    private StatsData stats;
    #region Health Properties
    public NetworkVariable<int> damageMultiplier = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> maxHealth = new NetworkVariable<int>(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> currentHealth = new NetworkVariable<int>(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> isDead = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public event Action<float, float> OnHealthChanged;
    public event Action OnDead;
    #endregion

    #region Setup
    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
        stats.OnStatReady += OnStatReady;
    }
    public void OnStatReady(StatsData _stats)
    {
        if (!IsServer) return;
        maxHealth.Value = Mathf.RoundToInt(stats.Health);
        currentHealth.Value = maxHealth.Value;
        isDead.Value = false;
        OnCurrentHealthChange(0, maxHealth.Value);
    }
    protected override void Start()
    {
        currentHealth.OnValueChanged += OnCurrentHealthChange;
        isDead.OnValueChanged += OnStateChanged;

    }
    #endregion

    #region Logic Health
    public virtual void DecreaseHealth(float damage, ulong attackerId)
    {
        if (!IsServer || ShouldDie())
            return;
        currentHealth.Value = Mathf.RoundToInt(Mathf.Max(0, currentHealth.Value - (damage * damageMultiplier.Value)));
        ShouldDie();
    }
    public virtual void IncreaseHealth()
    {
        if (!IsServer)
            return;
        if (currentHealth.Value > maxHealth.Value)
            return;
        currentHealth.Value++;

    }
    public bool ShouldDie()
    {
        if (currentHealth.Value <= 0)
        {
            isDead.Value = true;
            return true;
        }
        return false;
    }
    #endregion

    #region Callback
    public void OnStateChanged(bool previous, bool current)
    {
        if (previous == false && current == true)
        {
            OnDead?.Invoke();
        }
    }
    public void OnCurrentHealthChange(int previous, int current)
    {
        OnHealthChanged?.Invoke(current, maxHealth.Value);
    }
    #endregion

    protected override void LoadComponent()
    {
        base.LoadComponent();
        stats = GetComponent<StatsData>();
    }
}