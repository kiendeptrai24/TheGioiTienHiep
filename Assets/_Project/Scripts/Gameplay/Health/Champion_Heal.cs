
using System;
using DuloGames.UI.Tweens;
using UnityEngine;

public class Champion_Heal : TGTHMonoBehaviour, HealthController
{
    public int maxHealth;
    public int currentHealth;
    public int damageMultiplier = 1;
    private bool isDead = false;
    public event Action<float, float> OnHealthChanged;
    public event Action OnDead;

    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
    }
    public void Setup(int maxHealth, int currentHealth)
    {
        isDead = false;
        this.maxHealth = maxHealth;
        this.currentHealth = currentHealth;
        NotifyHealthChange();
    }
    public void DecreaseHealth(float damage, ulong attackerId)
    {
        if (ShouldDie())
            return;
        currentHealth = Mathf.RoundToInt(Mathf.Max(0, currentHealth - (damage * damageMultiplier)));
        NotifyHealthChange();
        ShouldDie();
    }
    private void NotifyHealthChange()
    {
        OnHealthChanged?.Invoke(maxHealth, currentHealth);
    }

    public float GetCurHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public void IncreaseHealth()
    {
        if (currentHealth > maxHealth)
            return;
        int previous = currentHealth;
        int current = currentHealth++;
        NotifyHealthChange();
    }
    public bool ShouldDie()
    {
        if (currentHealth <= 0)
        {
            bool previous = isDead;
            isDead = true;
            bool current = isDead;

            if (previous == false && current == true)
                OnDead?.Invoke();
            return true;
        }
        return false;
    }

    public void OnStatReady(StatsData _stats)
    {

    }
}