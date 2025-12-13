
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

[CreateAssetMenu(fileName = "NewStatsPreset", menuName = "RPG/Stats/Stats Realm Preset")]
public class StatsRealmPreset : ScriptableObject , IStatProvider
{
    [Header("Cultivation Realm")]
    public CultivationStage cultivationStage;
    [Header("Resources")]
    public Stat health;
    public Stat mana;
    public Stat spirit;

    [Header("Offensive Stats")]
    public Stat physicalDamage;
    public Stat magicalDamage;
    public Stat spiritDamage;
    public Stat critChance;
    public Stat critPower;

    [Header("Defensive Stats")]
    public Stat physicalDefense;
    public Stat magicalDefense;
    public Stat spiritDefense;
    public Stat evasion;
    public Stat spiritPenetration;

    [Header("Speed Stats")]
    public Stat movementSpeed;
    public Stat attackSpeed;
    public Stat castSpeed;

    [Header("Progression Stats")]
    public Stat potential;
    public Stat skillPoints;
    public Stat combatPower;
    [Header("Critical Stats")]
    public Stat spiritRange;
#if UNITY_EDITOR
    private void OnValidate() {
        string stageName = cultivationStage.ToString();
        if (string.IsNullOrEmpty(stageName))
            return;

        string newName = $"Stats_{stageName}";

        if (name != newName)
        {
            string path = AssetDatabase.GetAssetPath(this);
            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.RenameAsset(path, newName);
                AssetDatabase.SaveAssets();
            }
        }
        ResetToDefault();
    }
#endif
    [ContextMenu("Reset To Default")]
    public void ResetToDefault()
    {
        switch (cultivationStage)
        {
            // ===== C1 Luyện Khí =====
            case CultivationStage.LuyenKhi_1:
                ApplyBaseStats(100f, 100f, 100f, 10f, 10f, 10f,
                               5f, 5f, 5f,
                               10f, 4f, 5f, 3f, 10f);
                break;

            case CultivationStage.LuyenKhi_2:
                ApplyBaseStats(110f, 110f, 110f, 11f, 11f, 11f,
                               6f, 6f, 6f,
                               10f, 4f, 5f, 3f, 20f);
                break;

            case CultivationStage.LuyenKhi_3:
                ApplyBaseStats(120f, 120f, 120f, 12f, 12f, 12f,
                               6f, 6f, 6f,
                               10f, 4f, 5f, 3f, 40f);
                break;

            case CultivationStage.LuyenKhi_4:
                ApplyBaseStats(130f, 130f, 130f, 13f, 13f, 13f,
                               7f, 7f, 7f,
                               10f, 4f, 5f, 3f, 80f);
                break;

            case CultivationStage.LuyenKhi_5:
                ApplyBaseStats(140f, 140f, 140f, 14f, 14f, 14f,
                               7f, 7f, 7f,
                               10f, 4f, 5f, 3f, 150f);
                break;

            case CultivationStage.LuyenKhi_6:
                ApplyBaseStats(150f, 150f, 150f, 15f, 15f, 15f,
                               8f, 8f, 8f,
                               10f, 4f, 5f, 3f, 300f);
                break;

            case CultivationStage.LuyenKhi_7:
                ApplyBaseStats(160f, 160f, 160f, 16f, 16f, 16f,
                               8f, 8f, 8f,
                               10f, 4f, 5f, 3f, 500f);
                break;

            case CultivationStage.LuyenKhi_8:
                ApplyBaseStats(170f, 170f, 170f, 17f, 17f, 17f,
                               9f, 9f, 9f,
                               10f, 4f, 5f, 3f, 1000f);
                break;

            case CultivationStage.LuyenKhi_9:
                ApplyBaseStats(180f, 180f, 180f, 18f, 18f, 18f,
                               9f, 9f, 9f,
                               10f, 4f, 5f, 3f, 1400f);
                break;

            // ===== C2 Trúc Cơ =====
            case CultivationStage.TrucCo_SK:
                ApplyBaseStats(250f, 250f, 250f, 25f, 25f, 25f,
                               13f, 13f, 13f,
                               15f, 2f, 10f, 3f, 2000f);
                break;

            case CultivationStage.TrucCo_TK:
                ApplyBaseStats(300f, 300f, 300f, 30f, 30f, 30f,
                               15f, 15f, 15f,
                               15f, 2f, 10f, 3f, 2400f);
                break;

            case CultivationStage.TrucCo_HK:
                ApplyBaseStats(350f, 350f, 350f, 35f, 35f, 35f,
                               18f, 18f, 18f,
                               15f, 2f, 10f, 3f, 2600f);
                break;

            case CultivationStage.TrucCo_DVM:
                ApplyBaseStats(400f, 400f, 400f, 40f, 40f, 40f,
                               20f, 20f, 20f,
                               15f, 2f, 10f, 3f, 2800f);
                break;

            // ===== C3 Kết Đan =====
            case CultivationStage.KetDan_SK:
                ApplyBaseStats(500f, 500f, 500f, 50f, 50f, 50f,
                               25f, 25f, 25f,
                               20f, 3f, 20f, 3f, 3000f);
                break;

            case CultivationStage.KetDan_TK:
                ApplyBaseStats(600f, 600f, 600f, 60f, 60f, 60f,
                               30f, 30f, 30f,
                               20f, 3f, 20f, 3f, 3500f);
                break;

            case CultivationStage.KetDan_HK:
                ApplyBaseStats(700f, 700f, 700f, 70f, 70f, 70f,
                               35f, 35f, 35f,
                               20f, 3f, 20f, 3f, 4000f);
                break;

            case CultivationStage.KetDan_DVM:
                ApplyBaseStats(800f, 800f, 800f, 80f, 80f, 80f,
                               40f, 40f, 40f,
                               20f, 3f, 20f, 3f, 4500f);
                break;

            // ===== C4 Nguyên Anh =====
            case CultivationStage.NguyenAnh_SK:
                ApplyBaseStats(1000f, 1000f, 1000f, 100f, 100f, 100f,
                               50f, 50f, 50f,
                               30f, 4f, 80f, 3f, 5000f);
                break;

            case CultivationStage.NguyenAnh_TK:
                ApplyBaseStats(1500f, 1500f, 1500f, 150f, 150f, 150f,
                               75f, 75f, 75f,
                               30f, 4f, 80f, 3f, 6000f);
                break;

            case CultivationStage.NguyenAnh_HK:
                ApplyBaseStats(2000f, 2000f, 2000f, 200f, 200f, 200f,
                               100f, 100f, 100f,
                               30f, 4f, 80f, 3f, 7000f);
                break;

            case CultivationStage.NguyenAnh_DVM:
                ApplyBaseStats(2500f, 2500f, 2500f, 250f, 250f, 250f,
                               125f, 125f, 125f,
                               30f, 4f, 80f, 3f, 8000f);
                break;

            // ===== C5 Hóa Thần =====
            case CultivationStage.HoaThan_SK:
                ApplyBaseStats(3000f, 3000f, 3000f, 300f, 300f, 300f,
                               150f, 150f, 150f,
                               40f, 5f, 150f, 3f, 9000f);
                break;

            case CultivationStage.HoaThan_TK:
                ApplyBaseStats(4000f, 4000f, 4000f, 400f, 400f, 400f,
                               200f, 200f, 200f,
                               40f, 5f, 150f, 3f, 10000f);
                break;

            case CultivationStage.HoaThan_HK:
                ApplyBaseStats(5000f, 5000f, 5000f, 500f, 500f, 500f,
                               250f, 250f, 250f,
                               40f, 5f, 150f, 3f, 15000f);
                break;

            case CultivationStage.HoaThan_DVM:
                ApplyBaseStats(6000f, 6000f, 6000f, 600f, 600f, 600f,
                               300f, 300f, 300f,
                               40f, 5f, 150f, 3f, 20000f);
                break;

            // ===== C6 Hợp Thể =====
            case CultivationStage.HopThe_SK:
                ApplyBaseStats(7000f, 7000f, 7000f, 700f, 700f, 700f,
                               350f, 350f, 350f,
                               50f, 7f, 300f, 3f, 30000f);
                break;

            case CultivationStage.HopThe_TK:
                ApplyBaseStats(8500f, 8500f, 8500f, 850f, 850f, 850f,
                               425f, 425f, 425f,
                               50f, 7f, 300f, 3f, 50000f);
                break;

            case CultivationStage.HopThe_HK:
                ApplyBaseStats(10000f, 10000f, 10000f, 1000f, 1000f, 1000f,
                               500f, 500f, 500f,
                               50f, 7f, 300f, 3f, 100000f);
                break;

            case CultivationStage.HopThe_DVM:
                ApplyBaseStats(11500f, 11500f, 11500f, 1150f, 1150f, 1150f,
                               575f, 575f, 575f,
                               50f, 7f, 300f, 3f, 150000f);
                break;

            // ===== C7 Độ Kiếp =====
            case CultivationStage.DoKiep_SK:
                ApplyBaseStats(13000f, 13000f, 13000f, 1300f, 1300f, 1300f,
                               650f, 650f, 650f,
                               60f, 10f, 500f, 3f, 200000f);
                break;

            case CultivationStage.DoKiep_TK:
                ApplyBaseStats(15000f, 15000f, 15000f, 1500f, 1500f, 1500f,
                               750f, 750f, 750f,
                               60f, 10f, 500f, 3f, 300000f);
                break;

            case CultivationStage.DoKiep_HK:
                ApplyBaseStats(17000f, 17000f, 17000f, 1700f, 1700f, 1700f,
                               850f, 850f, 850f,
                               60f, 10f, 500f, 3f, 400000f);
                break;

            case CultivationStage.DoKiep_DVM:
                ApplyBaseStats(19000f, 19000f, 19000f, 1900f, 1900f, 1900f,
                               950f, 950f, 950f,
                               60f, 10f, 500f, 3f, 500000f);
                break;

            // ===== C8 Đại Thừa =====
            case CultivationStage.DaiThua_SK:
                ApplyBaseStats(25000f, 25000f, 25000f, 2500f, 2500f, 2500f,
                               1250f, 1250f, 1250f,
                               70f, 15f, 1000f, 3f, 800000f);
                break;

            case CultivationStage.DaiThua_TK:
                ApplyBaseStats(30000f, 30000f, 30000f, 3000f, 3000f, 3000f,
                               1500f, 1500f, 1500f,
                               70f, 15f, 1000f, 3f, 1000000f);
                break;

            case CultivationStage.DaiThua_HK:
                ApplyBaseStats(35000f, 35000f, 35000f, 3500f, 3500f, 3500f,
                               1750f, 1750f, 1750f,
                               70f, 15f, 1000f, 3f, 2000000f);
                break;

            case CultivationStage.DaiThua_DVM:
                ApplyBaseStats(40000f, 40000f, 40000f, 4000f, 4000f, 4000f,
                               2000f, 2000f, 2000f,
                               70f, 15f, 1000f, 3f, 5000000f);
                break;

            // ===== C9 Phi Thăng =====
            case CultivationStage.PhiThang:
                ApplyBaseStats(50000f, 50000f, 50000f, 5000f, 5000f, 5000f,
                               2500f, 2500f, 2500f,
                               100f, 20f, 2000f, 3f, 10000000f);
                break;
        }
    }

    private void ApplyBaseStats(
        float sinhLuc, float linhLuc, float linhThuc,
        float satThuongLinhThe, float satThuongLinhLuc, float satThuongLinhThuc,
        float phongNguLinhThe, float phongNguLinhLuc, float phongNguLinhThuc,
        float pvLinhThuc, float tocDoDiChuyen,
        float tiemNang, float phapKyDiem, float power)
    {

        health = new Stat(StatType.Health, sinhLuc);
        mana = new Stat(StatType.Mana, linhLuc);
        spirit = new Stat(StatType.Spirit, linhThuc);

        physicalDamage = new Stat(StatType.PhysicalDamage, satThuongLinhThe);
        magicalDamage = new Stat(StatType.MagicalDamage, satThuongLinhLuc);
        spiritDamage = new Stat(StatType.SpiritDamage, satThuongLinhThuc);

        physicalDefense = new Stat(StatType.PhysicalDefense, phongNguLinhThe);
        magicalDefense = new Stat(StatType.MagicalDefense, phongNguLinhLuc);
        spiritDefense = new Stat(StatType.SpiritDefense, phongNguLinhThuc);

        spiritPenetration = new Stat(StatType.SpiritRange, pvLinhThuc);
        movementSpeed = new Stat(StatType.MovementSpeed, tocDoDiChuyen);

        potential = new Stat(StatType.Potential, tiemNang);
        skillPoints = new Stat(StatType.SkillPoints, phapKyDiem);
        combatPower = new Stat(StatType.CombatPower, power);

        // các stats không có trong bảng: set default 1 lần, bạn tự chỉnh nếu muốn scale theo cảnh giới
        if (critChance == null) critChance = new Stat(StatType.CritChance, 5f);
        if (critPower == null) critPower = new Stat(StatType.CritPower, 150f);
        if (evasion == null) evasion = new Stat(StatType.Evasion, 2f);
        if (attackSpeed == null) attackSpeed = new Stat(StatType.AttackSpeed, 1f);
        if (castSpeed == null) castSpeed = new Stat(StatType.CastSpeed, 1f);
    }

    public void ApplyStats(Dictionary<StatType, Stat> stats)
    {
        stats.TryGetValue(StatType.Health, out Stat healthStat);
        stats.TryGetValue(StatType.Mana, out Stat manaStat);
        stats.TryGetValue(StatType.Spirit, out Stat spiritStat);
        stats.TryGetValue(StatType.PhysicalDamage, out Stat physicalDamageStat);
        stats.TryGetValue(StatType.MagicalDamage, out Stat magicalDamageStat);
        stats.TryGetValue(StatType.SpiritDamage, out Stat spiritDamageStat);
        stats.TryGetValue(StatType.PhysicalDefense, out Stat physicalDefenseStat);
        stats.TryGetValue(StatType.MagicalDefense, out Stat magicalDefenseStat);
        stats.TryGetValue(StatType.SpiritDefense, out Stat spiritDefenseStat);
        stats.TryGetValue(StatType.MovementSpeed, out Stat movementSpeedStat);
        stats.TryGetValue(StatType.SpiritRange, out Stat spiritRangeStat);
        stats.TryGetValue(StatType.Potential, out Stat potentialStat);
        stats.TryGetValue(StatType.SkillPoints, out Stat skillPointsStat);
        stats.TryGetValue(StatType.CombatPower, out Stat combatPowerStat);
        stats.TryGetValue(StatType.CritChance, out Stat critChanceStat);
        stats.TryGetValue(StatType.CritPower, out Stat critPowerStat);
        stats.TryGetValue(StatType.Evasion, out Stat evasionStat);
        stats.TryGetValue(StatType.AttackSpeed, out Stat attackSpeedStat);
        stats.TryGetValue(StatType.CastSpeed, out Stat castSpeedStat);

        healthStat.AddModifier(health.GetValue());
        manaStat.AddModifier(mana.GetValue());
        spiritStat.AddModifier(spirit.GetValue());
        physicalDamageStat.AddModifier(physicalDamage.GetValue());
        magicalDamageStat.AddModifier(magicalDamage.GetValue());
        spiritDamageStat.AddModifier(spiritDamage.GetValue());
        physicalDefenseStat.AddModifier(physicalDefense.GetValue());
        magicalDefenseStat.AddModifier(magicalDefense.GetValue());
        spiritDefenseStat.AddModifier(spiritDefense.GetValue());
        movementSpeedStat.AddModifier(movementSpeed.GetValue());
        spiritRangeStat.AddModifier(spiritRange.GetValue());
        potentialStat.AddModifier(potential.GetValue());
        skillPointsStat.AddModifier(skillPoints.GetValue());
        combatPowerStat.AddModifier(combatPower.GetValue());
        critChanceStat.AddModifier(critChance.GetValue());
        critPowerStat.AddModifier(critPower.GetValue());
        evasionStat.AddModifier(evasion.GetValue());
        attackSpeedStat.AddModifier(attackSpeed.GetValue());
        castSpeedStat.AddModifier(castSpeed.GetValue());
    }
    public StatsRealmData GetStats()
    {
        StatsRealmData data = new StatsRealmData();
        data.cultivationStage = cultivationStage;
        
        data.health = Mathf.RoundToInt(health.GetValue());
        data.mana = Mathf.RoundToInt(mana.GetValue());
        data.spirit = Mathf.RoundToInt(spirit.GetValue());

        data.physicalDamage = Mathf.RoundToInt(physicalDamage.GetValue());
        data.magicalDamage = Mathf.RoundToInt(magicalDamage.GetValue());
        data.spiritDamage = Mathf.RoundToInt(spiritDamage.GetValue());

        data.physicalDefense = Mathf.RoundToInt(physicalDefense.GetValue());
        data.magicalDefense = Mathf.RoundToInt(magicalDefense.GetValue());
        data.spiritDefense = Mathf.RoundToInt(spiritDefense.GetValue());
        data.movementSpeed = Mathf.RoundToInt(movementSpeed.GetValue());
        data.spiritRange = Mathf.RoundToInt(spiritRange.GetValue());
        data.potential = Mathf.RoundToInt(potential.GetValue());
        data.skillPoints = Mathf.RoundToInt(skillPoints.GetValue());
        data.combatPower = Mathf.RoundToInt(combatPower.GetValue());
        data.critChance = Mathf.RoundToInt(critChance.GetValue());
        data.critPower = Mathf.RoundToInt(critPower.GetValue());
        data.evasion = Mathf.RoundToInt(evasion.GetValue());
        data.attackSpeed = Mathf.RoundToInt(attackSpeed.GetValue());
        data.castSpeed = Mathf.RoundToInt(castSpeed.GetValue());
        data.spiritPenetration = Mathf.RoundToInt(spiritPenetration.GetValue());

        return data;
    }
}