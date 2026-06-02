
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using System;

[CreateAssetMenu(fileName = "NewStatsPreset", menuName = "RPG/Stats/Stats Realm Preset")]
public class StatsRealmPreset : ItemPreset
{
    [Header("Cultivation Realm")]
    public RealmType realmType;
    [Header("Resources")]
    public float mana;
    public float health;
    public float spirit;

    [Header("Offensive Stats")]
    public float physicalDamage;
    public float magicalDamage;
    public float spiritDamage;
    public float critChance;
    public float critPower;

    [Header("Defensive Stats")]
    public float physicalDefense;
    public float magicalDefense;
    public float spiritDefense;
    public float evasion;
    public float spiritPenetration;

    [Header("Speed Stats")]
    public float movementSpeed;
    public float attackSpeed;
    public float castSpeed;

    [Header("Progression Stats")]
    public float potential;
    public float skillPoints;
    public float combatPower;
    [Header("Critical Stats")]
    public float spiritRange;
    [Header("Upgrade Materials")]
    public float powerCost;              // Power
    public int linhthaoCost;              // Linh thảo
    public int khoangThachCost;            // Khoáng thạch
    public int yeuDanCost;          // Yêu đan
    public int mahachCost;          // Ma hạch
    public int linhThachCost;        // Linh thạch
    public int rewardPotentialPoint;
    public int rewardSkillPoint;


#if UNITY_EDITOR
    public override void OnValidate()
    {
        base.OnValidate();

        string stageName = realmType.ToString();
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
        switch (realmType)
        {
            // ===== C1 Luyện Khí =====
            case RealmType.LuyenKhi_1:
                ApplyBaseStats(100f, 100f, 100f, 10f, 10f, 10f,
                               5f, 5f, 5f,
                               10f, 4f, 5f, 3f, 10f);
                break;

            case RealmType.LuyenKhi_2:
                ApplyBaseStats(110f, 110f, 110f, 11f, 11f, 11f,
                               6f, 6f, 6f,
                               10f, 4f, 5f, 3f, 20f);
                break;

            case RealmType.LuyenKhi_3:
                ApplyBaseStats(120f, 120f, 120f, 12f, 12f, 12f,
                               6f, 6f, 6f,
                               10f, 4f, 5f, 3f, 40f);
                break;

            case RealmType.LuyenKhi_4:
                ApplyBaseStats(130f, 130f, 130f, 13f, 13f, 13f,
                               7f, 7f, 7f,
                               10f, 4f, 5f, 3f, 80f);
                break;

            case RealmType.LuyenKhi_5:
                ApplyBaseStats(140f, 140f, 140f, 14f, 14f, 14f,
                               7f, 7f, 7f,
                               10f, 4f, 5f, 3f, 150f);
                break;

            case RealmType.LuyenKhi_6:
                ApplyBaseStats(150f, 150f, 150f, 15f, 15f, 15f,
                               8f, 8f, 8f,
                               10f, 4f, 5f, 3f, 300f);
                break;

            case RealmType.LuyenKhi_7:
                ApplyBaseStats(160f, 160f, 160f, 16f, 16f, 16f,
                               8f, 8f, 8f,
                               10f, 4f, 5f, 3f, 500f);
                break;

            case RealmType.LuyenKhi_8:
                ApplyBaseStats(170f, 170f, 170f, 17f, 17f, 17f,
                               9f, 9f, 9f,
                               10f, 4f, 5f, 3f, 1000f);
                break;

            case RealmType.LuyenKhi_9:
                ApplyBaseStats(180f, 180f, 180f, 18f, 18f, 18f,
                               9f, 9f, 9f,
                               10f, 4f, 5f, 3f, 1400f);
                break;

            // ===== C2 Trúc Cơ =====
            case RealmType.TrucCo_SK:
                ApplyBaseStats(250f, 250f, 250f, 25f, 25f, 25f,
                               13f, 13f, 13f,
                               15f, 2f, 10f, 3f, 2000f);
                break;

            case RealmType.TrucCo_TK:
                ApplyBaseStats(300f, 300f, 300f, 30f, 30f, 30f,
                               15f, 15f, 15f,
                               15f, 2f, 10f, 3f, 2400f);
                break;

            case RealmType.TrucCo_HK:
                ApplyBaseStats(350f, 350f, 350f, 35f, 35f, 35f,
                               18f, 18f, 18f,
                               15f, 2f, 10f, 3f, 2600f);
                break;

            case RealmType.TrucCo_DVM:
                ApplyBaseStats(400f, 400f, 400f, 40f, 40f, 40f,
                               20f, 20f, 20f,
                               15f, 2f, 10f, 3f, 2800f);
                break;

            // ===== C3 Kết Đan =====
            case RealmType.KetDan_SK:
                ApplyBaseStats(500f, 500f, 500f, 50f, 50f, 50f,
                               25f, 25f, 25f,
                               20f, 3f, 20f, 3f, 3000f);
                break;

            case RealmType.KetDan_TK:
                ApplyBaseStats(600f, 600f, 600f, 60f, 60f, 60f,
                               30f, 30f, 30f,
                               20f, 3f, 20f, 3f, 3500f);
                break;

            case RealmType.KetDan_HK:
                ApplyBaseStats(700f, 700f, 700f, 70f, 70f, 70f,
                               35f, 35f, 35f,
                               20f, 3f, 20f, 3f, 4000f);
                break;

            case RealmType.KetDan_DVM:
                ApplyBaseStats(800f, 800f, 800f, 80f, 80f, 80f,
                               40f, 40f, 40f,
                               20f, 3f, 20f, 3f, 4500f);
                break;

            // ===== C4 Nguyên Anh =====
            case RealmType.NguyenAnh_SK:
                ApplyBaseStats(1000f, 1000f, 1000f, 100f, 100f, 100f,
                               50f, 50f, 50f,
                               30f, 4f, 80f, 3f, 5000f);
                break;

            case RealmType.NguyenAnh_TK:
                ApplyBaseStats(1500f, 1500f, 1500f, 150f, 150f, 150f,
                               75f, 75f, 75f,
                               30f, 4f, 80f, 3f, 6000f);
                break;

            case RealmType.NguyenAnh_HK:
                ApplyBaseStats(2000f, 2000f, 2000f, 200f, 200f, 200f,
                               100f, 100f, 100f,
                               30f, 4f, 80f, 3f, 7000f);
                break;

            case RealmType.NguyenAnh_DVM:
                ApplyBaseStats(2500f, 2500f, 2500f, 250f, 250f, 250f,
                               125f, 125f, 125f,
                               30f, 4f, 80f, 3f, 8000f);
                break;

            // ===== C5 Hóa Thần =====
            case RealmType.HoaThan_SK:
                ApplyBaseStats(3000f, 3000f, 3000f, 300f, 300f, 300f,
                               150f, 150f, 150f,
                               40f, 5f, 150f, 3f, 9000f);
                break;

            case RealmType.HoaThan_TK:
                ApplyBaseStats(4000f, 4000f, 4000f, 400f, 400f, 400f,
                               200f, 200f, 200f,
                               40f, 5f, 150f, 3f, 10000f);
                break;

            case RealmType.HoaThan_HK:
                ApplyBaseStats(5000f, 5000f, 5000f, 500f, 500f, 500f,
                               250f, 250f, 250f,
                               40f, 5f, 150f, 3f, 15000f);
                break;

            case RealmType.HoaThan_DVM:
                ApplyBaseStats(6000f, 6000f, 6000f, 600f, 600f, 600f,
                               300f, 300f, 300f,
                               40f, 5f, 150f, 3f, 20000f);
                break;

            // ===== C6 Hợp Thể =====
            case RealmType.HopThe_SK:
                ApplyBaseStats(7000f, 7000f, 7000f, 700f, 700f, 700f,
                               350f, 350f, 350f,
                               50f, 7f, 300f, 3f, 30000f);
                break;

            case RealmType.HopThe_TK:
                ApplyBaseStats(8500f, 8500f, 8500f, 850f, 850f, 850f,
                               425f, 425f, 425f,
                               50f, 7f, 300f, 3f, 50000f);
                break;

            case RealmType.HopThe_HK:
                ApplyBaseStats(10000f, 10000f, 10000f, 1000f, 1000f, 1000f,
                               500f, 500f, 500f,
                               50f, 7f, 300f, 3f, 100000f);
                break;

            case RealmType.HopThe_DVM:
                ApplyBaseStats(11500f, 11500f, 11500f, 1150f, 1150f, 1150f,
                               575f, 575f, 575f,
                               50f, 7f, 300f, 3f, 150000f);
                break;

            // ===== C7 Độ Kiếp =====
            case RealmType.DoKiep_SK:
                ApplyBaseStats(13000f, 13000f, 13000f, 1300f, 1300f, 1300f,
                               650f, 650f, 650f,
                               60f, 10f, 500f, 3f, 200000f);
                break;

            case RealmType.DoKiep_TK:
                ApplyBaseStats(15000f, 15000f, 15000f, 1500f, 1500f, 1500f,
                               750f, 750f, 750f,
                               60f, 10f, 500f, 3f, 300000f);
                break;

            case RealmType.DoKiep_HK:
                ApplyBaseStats(17000f, 17000f, 17000f, 1700f, 1700f, 1700f,
                               850f, 850f, 850f,
                               60f, 10f, 500f, 3f, 400000f);
                break;

            case RealmType.DoKiep_DVM:
                ApplyBaseStats(19000f, 19000f, 19000f, 1900f, 1900f, 1900f,
                               950f, 950f, 950f,
                               60f, 10f, 500f, 3f, 500000f);
                break;

            // ===== C8 Đại Thừa =====
            case RealmType.DaiThua_SK:
                ApplyBaseStats(25000f, 25000f, 25000f, 2500f, 2500f, 2500f,
                               1250f, 1250f, 1250f,
                               70f, 15f, 1000f, 3f, 800000f);
                break;

            case RealmType.DaiThua_TK:
                ApplyBaseStats(30000f, 30000f, 30000f, 3000f, 3000f, 3000f,
                               1500f, 1500f, 1500f,
                               70f, 15f, 1000f, 3f, 1000000f);
                break;

            case RealmType.DaiThua_HK:
                ApplyBaseStats(35000f, 35000f, 35000f, 3500f, 3500f, 3500f,
                               1750f, 1750f, 1750f,
                               70f, 15f, 1000f, 3f, 2000000f);
                break;

            case RealmType.DaiThua_DVM:
                ApplyBaseStats(40000f, 40000f, 40000f, 4000f, 4000f, 4000f,
                               2000f, 2000f, 2000f,
                               70f, 15f, 1000f, 3f, 5000000f);
                break;

            // ===== C9 Phi Thăng =====
            case RealmType.PhiThang:
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

        health = sinhLuc;
        mana = linhLuc;
        spirit = linhThuc;

        physicalDamage = satThuongLinhThe;
        magicalDamage = satThuongLinhLuc;
        spiritDamage = satThuongLinhThuc;

        physicalDefense = phongNguLinhThe;
        magicalDefense = phongNguLinhLuc;
        spiritDefense = phongNguLinhThuc;

        spiritPenetration = pvLinhThuc;
        movementSpeed = tocDoDiChuyen;

        potential = tiemNang;
        skillPoints = phapKyDiem;
        combatPower = power;

        // các stats không có trong bảng: set default 1 lần, bạn tự chỉnh nếu muốn scale theo cảnh giới
        critChance = 5f;
        critPower = 150f;
        evasion = 2f;
        attackSpeed = 1f;
        castSpeed = 1f;
    }

    public RealmData GetStats()
    {
        RealmData data = new RealmData();
        data.realmType = realmType;
        data.instanceId = instanceId;
        data.itemId = itemId;
        data.itemIcon = itemIcon;
        data.itemName = itemName;
        data.itemDescription = itemDescription;

        data.health = Mathf.RoundToInt(health);
        data.mana = Mathf.RoundToInt(mana);
        data.spirit = Mathf.RoundToInt(spirit);

        data.physicalDamage = Mathf.RoundToInt(physicalDamage);
        data.magicalDamage = Mathf.RoundToInt(magicalDamage);
        data.spiritDamage = Mathf.RoundToInt(spiritDamage);


        data.physicalDefense = Mathf.RoundToInt(physicalDefense);
        data.magicalDefense = Mathf.RoundToInt(magicalDefense);
        data.spiritDefense = Mathf.RoundToInt(spiritDefense);
        data.movementSpeed = Mathf.RoundToInt(movementSpeed);
        data.spiritRange = Mathf.RoundToInt(spiritRange);
        data.potential = Mathf.RoundToInt(potential);
        data.skillPoints = Mathf.RoundToInt(skillPoints);
        data.combatPower = Mathf.RoundToInt(combatPower);
        data.critDamage = Mathf.RoundToInt(critChance);
        data.critRate = Mathf.RoundToInt(critPower);
        data.evasion = Mathf.RoundToInt(evasion);
        data.attackSpeed = Mathf.RoundToInt(attackSpeed);
        data.castSpeed = Mathf.RoundToInt(castSpeed);
        data.armorPenetration = Mathf.RoundToInt(spiritPenetration);

        data.powerCost = powerCost;
        data.linhThaoCost = linhthaoCost;
        data.khoangThachCost = khoangThachCost;
        data.yeuDanCost = yeuDanCost;
        data.maHachCost = mahachCost;
        data.linhThachCost = linhThachCost;
        data.rewardPotentialPoint = rewardPotentialPoint;
        data.rewardPotentialPoint = rewardPotentialPoint;
        data.rewardSkillPoint = rewardSkillPoint;


        return data;
    }
}