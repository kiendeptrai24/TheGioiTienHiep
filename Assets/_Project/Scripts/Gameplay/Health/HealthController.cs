using System;
using Unity.Netcode;
using UnityEngine;

public interface HealthController
{
    public float GetMaxHealth();
    public float GetCurHealth();
    #region Health Properties
    public event Action<float, float> OnHealthChanged;
    public event Action OnDead;
    #endregion

    #region Setup
    public void OnStatReady(StatsData _stats);
    #endregion

    #region Logic Health
    public void DecreaseHealth(float damage, ulong attackerId);
    public void IncreaseHealth();
    public bool ShouldDie();
    #endregion
}