using System;
using UnityEngine;

public class Champion_Heal : TGTHMonoBehaviour, HealthController
{
    [SerializeField] private VitalValue health = new();
    [SerializeField] private VitalValue mana = new();
    [SerializeField] private VitalValue spirit = new();

    public int damageMultiplier = 1;
    private bool isDead;

    public event Action<float, float> OnHealthChanged;
    public event Action<float, float> OnManaChanged;
    public event Action<float, float> OnSpiritChanged;
    public event Action OnDead;

    public void Setup(
        int maxHealth, int currentHealth,
        int maxMana, int currentMana,
        int maxSpirit, int currentSpirit)
    {
        isDead = false;

        health.Set(maxHealth, currentHealth);
        mana.Set(maxMana, currentMana);
        spirit.Set(maxSpirit, currentSpirit);

        NotifyAllChanged();
    }

    public void DecreaseHealth(float damage, ulong attackerId)
    {
        if (isDead) return;

        int finalDamage = Mathf.RoundToInt(damage * damageMultiplier);
        health.Decrease(finalDamage);

        NotifyHealthChanged();
        CheckDead();
    }

    public void IncreaseHealth(int amount)
    {
        if (isDead) return;

        health.Increase(amount);
        NotifyHealthChanged();
    }

    public void IncreaseMana(int amount)
    {
        mana.Increase(amount);
        NotifyManaChanged();
    }

    public void DecreaseMana(int amount)
    {
        mana.Decrease(amount);
        NotifyManaChanged();
    }

    public void IncreaseSpirit(int amount)
    {
        spirit.Increase(amount);
        NotifySpiritChanged();
    }

    public void DecreaseSpirit(int amount)
    {
        spirit.Decrease(amount);
        NotifySpiritChanged();
    }

    public float GetCurHealth() => health.Current;
    public float GetMaxHealth() => health.Max;
    public float GetCurMana() => mana.Current;
    public float GetMaxMana() => mana.Max;
    public float GetCurSpirit() => spirit.Current;
    public float GetMaxSpirit() => spirit.Max;

    public VitalValue GetVital(VitalType type)
    {
        return type switch
        {
            VitalType.Health => health,
            VitalType.Mana => mana,
            VitalType.Spirit => spirit,
            _ => health
        };
    }

    private void CheckDead()
    {
        if (isDead) return;
        if (health.Current > 0) return;

        isDead = true;
        OnDead?.Invoke();
    }

    private void NotifyAllChanged()
    {
        NotifyHealthChanged();
        NotifyManaChanged();
        NotifySpiritChanged();
    }

    private void NotifyHealthChanged()
    {
        OnHealthChanged?.Invoke(health.Max, health.Current);
    }

    private void NotifyManaChanged()
    {
        OnManaChanged?.Invoke(mana.Max, mana.Current);
    }

    private void NotifySpiritChanged()
    {
        OnSpiritChanged?.Invoke(spirit.Max, spirit.Current);
    }

    public void OnStatReady(StatsData stats)
    {
        Setup(
            stats.Health, stats.Health,
            stats.Mana, stats.Mana,
            stats.Spirit, stats.Spirit
        );
    }

    public void IncreaseHealth()
    {
    }

    public bool ShouldDie()
    {
        if (health.Current <= 0)
        {
            bool previous = isDead; isDead = true;
            bool current = isDead;
            if (previous == false && current == true)
                OnDead?.Invoke(); return true;
        }
        return false;
    }
}