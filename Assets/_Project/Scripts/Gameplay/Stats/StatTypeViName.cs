using System;
using System.Collections.Generic;

public static class StatTypeViName
{
    private static readonly Dictionary<StatType, string> _viNames = new Dictionary<StatType, string>
    {
        { StatType.Health, "Sinh lực" },
        { StatType.Mana, "Linh lực" },
        { StatType.Spirit, "Linh thức" },
        { StatType.PhysicalDamage, "Sát thương vật lý" },
        { StatType.MagicalDamage, "Sát thương phép" },
        { StatType.SpiritDamage, "Sát thương tinh thần" },
        { StatType.CritChance, "Tỷ lệ chí mạng" },
        { StatType.CritPower, "Sát thương chí mạng" },
        { StatType.TrueDamage, "Sát thương chuẩn" },
        { StatType.ArmorPenetration, "Xuyên phòng ngự" },
        { StatType.SpiritPenetration, "Xuyên phòng ngự tinh thần" },
        { StatType.LifeSteal, "Hút sinh lực" },
        { StatType.PhysicalDefense, "Giáp vật lý" },
        { StatType.MagicalDefense, "Kháng phép" },
        { StatType.SpiritDefense, "Kháng tinh thần" },
        { StatType.ReflectDamage, "Phản đòn" },
        { StatType.CritDamageReduction, "Giảm sát thương chí mạng" },
        { StatType.PenetrationDamageReduction, "Giảm sát thương xuyên giáp" },
        { StatType.TrueDamageReduction, "Giảm sát thương chuẩn" },
        { StatType.HealthRegen, "Hồi sinh lực" },
        { StatType.ManaRegen, "Hồi linh lực" },
        { StatType.SpiritRegen, "Hồi linh thức" },
        { StatType.AllyHealthRegen, "Hồi máu đồng minh" },
        { StatType.AllyManaRegen, "Hồi năng lượng đồng minh" },
        { StatType.AllySpiritRegen, "Hồi thần thức đồng minh" },
        { StatType.DamageImmunity, "Miễn sát thương" },
        { StatType.CCImmunity, "Miễn hiệu ứng" },
        { StatType.FullImmunity, "Miễn tất cả" },
        { StatType.AllyDamageImmunity, "Miễn sát thương đồng minh" },
        { StatType.AllyCCImmunity, "Miễn hiệu ứng đồng minh" },
        { StatType.AllyFullImmunity, "Miễn tất cả đồng minh" },
        { StatType.AllyCleanse, "Giải trừ hiệu ứng đồng đội" },
        { StatType.HealingReduction, "Vết thương sâu" },
        { StatType.EnemyManaReduction, "Giảm linh lực đối phương" },
        { StatType.EnemySpiritReduction, "Giảm linh thức đối phương" },
        { StatType.Weaken, "Suy yếu" },
        { StatType.Paralyze, "Tê liệt" },
        { StatType.Root, "Vây khốn" },
        { StatType.Stun, "Choáng" },
        { StatType.Silence, "Câm lặng" },
        { StatType.CCDurationReduction, "Giảm thời gian hiệu ứng" },
        { StatType.CCResistance, "Kháng hiệu ứng" },
        { StatType.MovementSpeed, "Tốc độ di chuyển" },
        { StatType.AttackSpeed, "Tốc độ đánh" },
        { StatType.CastSpeed, "Tốc độ thi triển" },
        { StatType.PotentialPoint, "Điểm tiềm năng" },
        { StatType.SkillPoint, "Điểm kỹ năng" },
        { StatType.CombatPower, "Lực chiến" },
        { StatType.SpiritRange, "Tầm linh thức" },
        { StatType.Evasion, "Né tránh" },
        { StatType.CounterPercentage, "Tỷ lệ khắc chế" },
        { StatType.AttackRange, "Tầm đánh" },
        { StatType.BonusHealth, "Tăng sinh lực" },
        { StatType.BonusMana, "Tăng linh lực" },
        { StatType.BonusSpirit, "Tăng linh thức" },
    };

    public static string ToVietnamese(StatType statType)
    {
        if (_viNames.TryGetValue(statType, out var name))
            return name;
        return statType.ToString();
    }
}
