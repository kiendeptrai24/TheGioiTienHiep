using TGTH.Mobile;
using UnityEngine;

public abstract class StatsSystem : TGTHMonoBehaviour
{
    // private CharacterStats charStats;
    [SerializeField] private StatsManager statsManager;
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

    protected override void LoadComponent()
    {
        base.LoadComponent();
        statsManager = GetComponent<StatsManager>();
    }
}
