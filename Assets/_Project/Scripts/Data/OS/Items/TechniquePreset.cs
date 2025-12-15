#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(fileName = "NewTechniquePreset", menuName = "RPG/Items/Technique Preset")]
public class TechniquePreset : ItemPreset
{
    // ============================
    // META INFO
    // ============================
    [Header("Damage Stats")]
    public float physicalDamage;
    public float magicalDamage;
    public float spiritDamage;
    public float physicalDefense;
    public float magicalDefense;
    public float spiritDefense;

    public TechniqueType techniqueType;
    public QualityType qualityType;     // Phẩm
    public int enhanceLevel;             // Cường hóa
    public RaceType raceType;            // Tộc
    public EssenceType mainEssence;      // Chủ tu
    public ElementType elementType;      // Hệ
    public CultivationStage realm;       // Cảnh giới

    // ============================
    // COMBAT BEHAVIOR
    // ============================
    [Header("Combat")]
    public float attackRange;            // Tầm đánh
    public float cooldown;               // Cooldown
    public string specialEffect;         // Hiệu ứng (mô tả / key effect)

    // ============================
    // RESOURCE COST
    // ============================
    [Header("Resource Cost")]
    public float healthCost;             // Tiêu hao sinh lực
    public float manaCost;               // Tiêu hao linh lực
    public float spiritCost;             // Tiêu hao linh thức

    // ============================
    // LEARN CONDITIONS
    // ============================
    [Header("Learn Conditions")]
    public int requiredCharacterLevel;   // Cấp nhân vật
    public string learnCondition;         // Điều kiện học (text / key)

    // ============================
    // CONSUME MATERIALS
    // ============================
    [Header("Upgrade Materials")]
    public float powerCost;               // Power
    public float lthaoCost;               // Linh thảo
    public float mineralCost;             // Khoáng thạch
    public float demonCoreCost;           // Yêu đan
    public float devilCoreCost;           // Ma hạch
    public float spiritStoneCost;         // Linh thạch
    public float itemCost;                // Vật phẩm khác

    // ============================
    // OFFENSIVE STATS BONUS
    // ============================
    [Header("Damage Bonus")]
    public float critDamage;              // Sát thương chí mạng
    public float critRate;                // Tỷ lệ chí mạng

    public float armorPenetration;        // Xuyên phòng ngự
    public float trueDamage;              // Sát thương chuẩn
    public float lifeSteal;               // Hút sinh lực
    public float attackSpeed;             // Tốc độ đánh

    // ============================
    // DEFENSIVE STATS BONUS
    // ============================
    [Header("Defense Bonus")]

    public float penetrationReduction;   // Giảm sát thương xuyên phòng ngự
    public float critDamageReduction;     // Giảm sát thương chí mạng
    public float trueDamageReduction;     // Giảm sát thương chuẩn

    // ============================
    // RESOURCE BONUS
    // ============================
    [Header("Resource Bonus")]
    public float bonusHealth;             // Tăng sinh lực
    public float bonusMana;               // Tăng linh lực
    public float bonusSpirit;             // Tăng linh thức

    // ============================
    // SUMMARY / CALCULATION INFO
    // ============================
    [Header("Summary")]
    public float totalQualityAndLevel;    // Tổng (phẩm + cấp)
    public int statCount;                 // Số chỉ số kích hoạt
    [ContextMenu("Reset To Default")]
    public void ResetToDefault()
    {
        ClearStats();

        switch (techniqueType)
        {
            // =====================================================
            // PHÀM NHÂN LUYỆN LINH QUYẾT
            // =====================================================
            case TechniqueType.PhamNhanLuyenLinhQuyet:
                ApplyPhamNhanLuyenLinh();
                break;

            // =====================================================
            // LINH VẬN QUYẾT
            // =====================================================
            case TechniqueType.LinhVanQuyet:
                ApplyLinhVan();
                break;

            // =====================================================
            // YÊU LINH QUYẾT
            // =====================================================
            case TechniqueType.YeuLinhQuyet:
                ApplyYeuLinh();
                break;

            // =====================================================
            // MAN NGƯU BÍ PHÁP
            // =====================================================
            case TechniqueType.ManNguuBiPhap:
                ApplyManNguu();
                break;
        }
#if UNITY_EDITOR
        string newName = $"Technique_{itemName}";
        if (name != newName)
        {
            string path = AssetDatabase.GetAssetPath(this);
            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.RenameAsset(path, newName);
                AssetDatabase.SaveAssets();
            }
        }
#endif
    }
    private void ClearStats()
    {
        bonusHealth = bonusMana = bonusSpirit = 0;

        physicalDamage = magicalDamage = spiritDamage = 0;
        physicalDefense = magicalDefense = spiritDefense = 0;

        critDamage = critRate = 0;
        armorPenetration = trueDamage = lifeSteal = attackSpeed = 0;

        penetrationReduction = critDamageReduction = trueDamageReduction = 0;

        statCount = 0;
        totalQualityAndLevel = 0;
    }
    private void ApplyPhamNhanLuyenLinh()
    {
        switch (realm)
        {
            case CultivationStage.LuyenKhi_9:
                bonusHealth = 20;
                bonusMana   = 20;
                enhanceLevel = 1;
                break;

            case CultivationStage.TrucCo_SK:
                bonusHealth = 30;
                bonusMana   = 30;
                enhanceLevel = 2;
                break;

            case CultivationStage.KetDan_SK:
                bonusHealth = 50;
                bonusMana   = 50;
                enhanceLevel = 3;
                break;

            case CultivationStage.NguyenAnh_SK:
                bonusHealth = 70;
                bonusMana   = 70;
                enhanceLevel = 4;
                break;

            case CultivationStage.HoaThan_SK:
                bonusHealth = 90;
                bonusMana   = 90;
                enhanceLevel = 5;
                break;

            case CultivationStage.HopThe_SK:
                bonusHealth = 110;
                bonusMana   = 110;
                enhanceLevel = 6;
                break;

            case CultivationStage.DoKiep_SK:
                bonusHealth = 130;
                bonusMana   = 130;
                enhanceLevel = 7;
                break;

            case CultivationStage.DaiThua_SK:
                bonusHealth = 160;
                bonusMana   = 160;
                enhanceLevel = 8;
                break;

            case CultivationStage.PhiThang:
                bonusHealth = 210;
                bonusMana   = 210;
                enhanceLevel = 9;
                break;
        }
        itemName = $"Phàm Nhân Luyện Linh Quyết C{enhanceLevel}";
        statCount = 2;
        totalQualityAndLevel = bonusHealth * 0.5f;
    }
    private void ApplyLinhVan()
    {
        switch (realm)
        {
            case CultivationStage.LuyenKhi_9:
                physicalDefense = 5;
                magicalDefense  = 5;
                bonusHealth     = 20;
                enhanceLevel = 1;
                break;

            case CultivationStage.TrucCo_SK:
                physicalDefense = 7.5f;
                magicalDefense  = 7.5f;
                bonusHealth     = 30;
                enhanceLevel = 2;
                break;

            case CultivationStage.KetDan_SK:
                physicalDefense = 12.5f;
                magicalDefense  = 12.5f;
                bonusHealth     = 50;
                enhanceLevel = 3;
                break;
        }

        itemName = $"Linh Vận Quyết C{enhanceLevel}";
        statCount = 3;
    }
    private void ApplyYeuLinh()
    {
        switch (realm)
        {
            case CultivationStage.LuyenKhi_9:
                magicalDamage = 10;
                bonusHealth   = 20;
                enhanceLevel = 1;
                break;

            case CultivationStage.TrucCo_SK:
                magicalDamage = 15;
                bonusHealth   = 30;
                enhanceLevel = 2;
                break;

            case CultivationStage.KetDan_SK:
                magicalDamage = 25;
                bonusHealth   = 50;
                enhanceLevel = 3;
                break;
        }

        itemName = $"Yêu Linh Quyết C{enhanceLevel}";
        statCount = 2;
    }
    private void ApplyManNguu()
    {

        switch (realm)
        {
            case CultivationStage.LuyenKhi_9:
                physicalDamage  = 5;
                physicalDefense = 5;
                bonusHealth     = 20;
                enhanceLevel = 1;
                break;

            case CultivationStage.TrucCo_SK:
                physicalDamage  = 10;
                physicalDefense = 5;
                bonusHealth     = 30;
                enhanceLevel = 2;
                break;

            case CultivationStage.KetDan_SK:
                physicalDamage  = 20;
                physicalDefense = 5;
                bonusHealth     = 50;
                enhanceLevel = 3;
                break;
        }

        itemName = $"Man Ngưu Bí Pháp C{enhanceLevel}";
        statCount = 3;
    }



}