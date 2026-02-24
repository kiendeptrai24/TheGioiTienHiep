
using System;
using DuloGames.UI.Tweens;
using UnityEngine;

public class Champion_Heal : TGTHMonoBehaviour, HealthController
{
    private StatsData stats;
    public int maxHealth;
    public int currentHealth;
    public int damageMultiplier = 1;
    private bool isDead = false;
    public event Action<float, float> OnHealthChanged;
    public event Action OnDead;

    protected override void Awake()
    {
        base.Awake();
        stats = GetComponent<StatsData>();
        stats.OnStatReady += OnStatReady;
    }
    override protected void Start()
    {
        base.Start();
        stats.SetupDataPreset();
    }
    public void DecreaseHealth(float damage, ulong attackerId)
    {
        Debug.Log("Champion Decrease Health: " + currentHealth + "/" + maxHealth);
        if (ShouldDie())
            return;
        currentHealth = Mathf.RoundToInt(Mathf.Max(0, currentHealth - (damage * damageMultiplier)));
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        ShouldDie();
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
        OnHealthChanged?.Invoke(current, maxHealth);
    }
    public void OnStatReady(StatsData _stats)
    {
        maxHealth = Mathf.RoundToInt(stats.Health);
        currentHealth = maxHealth;
        isDead = false;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
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
}