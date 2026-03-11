using TGTH.Mobile;
using UnityEngine;

public abstract class StatsSystem : TGTHMonoBehaviour
{
    // private CharacterStats charStats;
    [SerializeField] private StatsData statsManager;
    protected override void Awake()
    {
        LoadComponent();
    }
    public abstract void Equip(InventoryItem item);
    public abstract void Unequip(InventoryItem item);

    protected void AddPercent(StatType type, float percent)
    {
        if (percent == 0) return;

        if (statsManager.stats.TryGetValue(type, out Stat stat))
            stat.AddModifierPercent(percent);
    }
    protected void RemovePercent(StatType type, float percent)
    {
        if (percent == 0) return;

        if (statsManager.stats.TryGetValue(type, out Stat stat))
            stat.RemoveModifierPercent(percent);
    }
    protected void AddValue(StatType type, float value)
    {
        if (value == 0) return;
        if (statsManager.stats.TryGetValue(type, out Stat stat))
            stat.AddModifier(value);
    }
    protected void RemoveValue(StatType type, float value)
    {
        if (value == 0) return;
        if (statsManager.stats.TryGetValue(type, out Stat stat))
            stat.RemoveModifier(value);
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
    }
}
