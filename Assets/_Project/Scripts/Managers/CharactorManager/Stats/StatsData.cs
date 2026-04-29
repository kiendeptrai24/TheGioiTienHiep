

using System;
using System.Collections.Generic;
using UnityEngine;

public class StatsData : TGTHMonoBehaviour
{
    public event Action OnValueChanged;
    public HeroPreset heroPreset;

    public ItemData heroData;
    public List<TechniqueData> techniqueData;
    public List<SkillData> skillDatas;
    public List<EquitmentData> equiDatas;

    [Header("Preset base stats")]
    public RaceData statsRaceData;
    public EssenceData statsCultivationPathData;
    public RealmData statsRealmData;
    public Dictionary<StatType, Stat> stats = new Dictionary<StatType, Stat>();

    #region Base Stats
    public int Health => GetStatValue(StatType.Health);
    public int Mana => GetStatValue(StatType.Mana);
    public int Spirit => GetStatValue(StatType.Spirit);
    #endregion
    #region Damage
    public int PhysicalDamage => GetStatValue(StatType.PhysicalDamage);
    public int MagicalDamage => GetStatValue(StatType.MagicalDamage);
    public int SpiritDamage => GetStatValue(StatType.SpiritDamage);

    public float TrueDamage => GetStatValue(StatType.TrueDamage);
    public float ArmorPenetration => GetStatValue(StatType.ArmorPenetration);
    public float SpiritPenetration => GetStatValue(StatType.SpiritPenetration);
    public float LifeSteal => GetStatValue(StatType.LifeSteal);
    #endregion
    #region Defense & Damage Reduction
    public int PhysicalDefense => GetStatValue(StatType.PhysicalDefense);
    public int MagicalDefense => GetStatValue(StatType.MagicalDefense);
    public int SpiritDefense => GetStatValue(StatType.SpiritDefense);

    public float ReflectDamage => GetStatValue(StatType.ReflectDamage);

    public float CritDamageReduction => GetStatValue(StatType.CritDamageReduction);
    public float PenetrationDamageReduction => GetStatValue(StatType.PenetrationDamageReduction);
    public float TrueDamageReduction => GetStatValue(StatType.TrueDamageReduction);
    #endregion
    #region Regen & Ally Regen
    public float HealthRegen => GetStatValue(StatType.HealthRegen);
    public float ManaRegen => GetStatValue(StatType.ManaRegen);
    public float SpiritRegen => GetStatValue(StatType.SpiritRegen);

    public float AllyHealthRegen => GetStatValue(StatType.AllyHealthRegen);
    public float AllyManaRegen => GetStatValue(StatType.AllyManaRegen);
    public float AllySpiritRegen => GetStatValue(StatType.AllySpiritRegen);
    #endregion
    #region Immunity & Ally Immunity
    public float DamageImmunity => GetStatValue(StatType.DamageImmunity);
    public float CCImmunity => GetStatValue(StatType.CCImmunity);
    public float FullImmunity => GetStatValue(StatType.FullImmunity);

    public float AllyDamageImmunity => GetStatValue(StatType.AllyDamageImmunity);
    public float AllyCCImmunity => GetStatValue(StatType.AllyCCImmunity);
    public float AllyFullImmunity => GetStatValue(StatType.AllyFullImmunity);
    #endregion
    #region Debuff
    public float HealingReduction => GetStatValue(StatType.HealingReduction);
    public float EnemyManaReduction => GetStatValue(StatType.EnemyManaReduction);
    public float EnemySpiritReduction => GetStatValue(StatType.EnemySpiritReduction);
    #endregion
    #region CC / Debuff
    public float Weaken => GetStatValue(StatType.Weaken);
    public float Paralyze => GetStatValue(StatType.Paralyze);
    public float Root => GetStatValue(StatType.Root);
    public float Stun => GetStatValue(StatType.Stun);
    public float Silence => GetStatValue(StatType.Silence);
    #endregion
    #region reaction and reduction of effects
    public float CCDurationReduction => GetStatValue(StatType.CCDurationReduction);
    public float CCResistance => GetStatValue(StatType.CCResistance);

    #endregion
    #region Speed
    public int MovementSpeed => GetStatValue(StatType.MovementSpeed);
    public int AttackSpeed => GetStatValue(StatType.AttackSpeed);
    public int CastSpeed => GetStatValue(StatType.CastSpeed);
    #endregion
    #region Range
    public int AttackRange => GetStatValue(StatType.AttackRange);
    public int SpiritRange => GetStatValue(StatType.SpiritRange);
    #endregion

    public int CombatPower => GetStatValue(StatType.CombatPower);
    public event Action<StatsData> OnStatReady;
    public List<IStatsModifier> statsModifiers;

    protected override void Awake()
    {
        base.Awake();
        InitStatsModifier();
        Setup();
    }
    public void InitStatsModifier()
    {
        statsModifiers = new();
        statsModifiers.Add(new StatsCharacterModifier());
        statsModifiers.Add(new StatsRealmModifier());
        statsModifiers.Add(new StatsCultivationPathModifier());
        statsModifiers.Add(new StatsRaceModifier());
        // statsModifiers.Add(new StatsEquipmentModifier());
        // statsModifiers.Add(new StatsTechniqueModifier());
        // statsModifiers.Add(new StatsSkillModifier());
        // statsModifiers.Add(new StatsPointModifier());
    }
    private void ResetStatsModifiers()
    {
        stats.Clear();
        foreach (StatType type in Enum.GetValues(typeof(StatType)))
        {
            stats.Add(type, new Stat(type, 0f));
        }
    }
    public void ResetStats()
    {
        ResetStatsModifiers();
        StatChange();
    }

    #region Stats Emplementation
    public int GetStatValue(StatType type)
    {
        if (stats.TryGetValue(type, out Stat stat))
        {
            return Mathf.RoundToInt(stat.GetValue());
        }

        Debug.LogWarning($"Stat {type} không tồn tại trên {name}!");
        return 0;
    }
    public Stat GetStat(StatType type)
    {
        if (stats.TryGetValue(type, out Stat stat))
        {
            return stat;
        }
        return null;
    }
    [ContextMenu("Show Stats")]
    private void ShowStas()
    {
        string debugMsg = $"{name} Stats:\n";
        foreach (var stat in stats)
        {
            debugMsg += $"{stat.Key}: {stat.Value.GetValue()}\n";
        }
        Debug.Log(debugMsg);
    }
    #endregion
    #region Setup Item Data
    private void Setup()
    {
        if (statsModifiers == null) InitStatsModifier();
        ResetStats();
        foreach (var modifier in statsModifiers)
        {
            modifier.AddStats(stats, heroData);
        }
        StatChange();
        SetupDebbug();
        OnStatReady?.Invoke(this);
    }
    private void SetupDebbug()
    {
        var hero = heroData as HeroData;
        if (hero == null) return;
        techniqueData = hero.techniqueDatas;
        skillDatas = hero.skillDatas;
        equiDatas = hero.equipmentDatas;

        statsRaceData = hero.raceData;
        statsRealmData = hero.realmData;
        statsCultivationPathData = hero.essenceData;
    }
    public void SetUpItem(ItemData item)
    {
        this.heroData = item;
        Setup();
    }
    public void SetupDataPreset()
    {
        if (heroPreset == null) return;
        SetUpItem(heroPreset.GetItemData());
    }
    #endregion

    public void StatChange()
    {
        OnValueChanged?.Invoke();
    }
}