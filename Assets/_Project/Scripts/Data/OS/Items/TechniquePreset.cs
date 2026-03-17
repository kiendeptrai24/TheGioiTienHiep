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
    public int enhanceLevel;             // Cường hóa
    public RaceType raceType;            // Tộc
    public EssenceType mainEssence;      // Chủ tu
    public ElementType elementType;      // Hệ
    public RealmType realm;       // Cảnh giới

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
            case RealmType.LuyenKhi_9:
                bonusHealth = .20f;
                bonusMana = .20f;
                enhanceLevel = 1;
                break;

            case RealmType.TrucCo_SK:
                bonusHealth = .30f;
                bonusMana = .30f;
                enhanceLevel = 2;
                break;

            case RealmType.KetDan_SK:
                bonusHealth = .50f;
                bonusMana = .50f;
                enhanceLevel = 3;
                break;

            case RealmType.NguyenAnh_SK:
                bonusHealth = .70f;
                bonusMana = .70f;
                enhanceLevel = 4;
                break;

            case RealmType.HoaThan_SK:
                bonusHealth = .90f;
                bonusMana = .90f;
                enhanceLevel = 5;
                break;

            case RealmType.HopThe_SK:
                bonusHealth = 1.10f;
                bonusMana = 1.10f;
                enhanceLevel = 6;
                break;

            case RealmType.DoKiep_SK:
                bonusHealth = 1.30f;
                bonusMana = 1.30f;
                enhanceLevel = 7;
                break;

            case RealmType.DaiThua_SK:
                bonusHealth = 1.60f;
                bonusMana = 1.60f;
                enhanceLevel = 8;
                break;

            case RealmType.PhiThang:
                bonusHealth = 2.10f;
                bonusMana = 2.10f;
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
            case RealmType.LuyenKhi_9:
                physicalDefense = .05f;
                magicalDefense = .05f;
                bonusHealth = .20f;
                enhanceLevel = 1;
                break;

            case RealmType.TrucCo_SK:
                physicalDefense = .075f;
                magicalDefense = .075f;
                bonusHealth = .30f;
                enhanceLevel = 2;
                break;

            case RealmType.KetDan_SK:
                physicalDefense = .125f;
                magicalDefense = .125f;
                bonusHealth = .50f;
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
            case RealmType.LuyenKhi_9:
                magicalDamage = .10f;
                bonusHealth = .20f;
                enhanceLevel = 1;
                break;

            case RealmType.TrucCo_SK:
                magicalDamage = .15f;
                bonusHealth = .30f;
                enhanceLevel = 2;
                break;

            case RealmType.KetDan_SK:
                magicalDamage = .25f;
                bonusHealth = .50f;
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
            case RealmType.LuyenKhi_9:
                physicalDamage = .05f;
                physicalDefense = .05f;
                bonusHealth = .20f;
                enhanceLevel = 1;
                break;

            case RealmType.TrucCo_SK:
                physicalDamage = .10f;
                physicalDefense = .05f;
                bonusHealth = .30f;
                enhanceLevel = 2;
                break;

            case RealmType.KetDan_SK:
                physicalDamage = .20f;
                physicalDefense = .05f;
                bonusHealth = .50f;
                enhanceLevel = 3;
                break;
        }

        itemName = $"Man Ngưu Bí Pháp C{enhanceLevel}";
        statCount = 3;
    }

    public override ItemData GetItemData()
    {
        ItemData data = base.GetItemData();
        return new TechniqueData
        {
            // base
            itemId = data.itemId,
            itemName = data.itemName,
            itemType = data.itemType,
            itemIcon = data.itemIcon,
            itemIconPath = data.itemIconPath,
            itemDescription = data.itemDescription,
            currentstack = data.currentstack,
            canStack = data.canStack,
            itemPrice = data.itemPrice,
            realmType = data.realmType,
            qualityType = data.qualityType,

            // base stats trong ItemData
            physicalDamage = physicalDamage,
            magicalDamage = magicalDamage,
            spiritDamage = spiritDamage,
            physicalDefense = physicalDefense,
            magicalDefense = magicalDefense,
            spiritDefense = spiritDefense,

            // meta
            enhanceLevel = enhanceLevel,
            raceType = raceType,
            mainEssence = mainEssence,
            elementType = elementType,
            realm = realm,

            // combat
            attackRange = attackRange,
            cooldown = cooldown,
            specialEffect = specialEffect,

            // costs
            healthCost = healthCost,
            manaCost = manaCost,
            spiritCost = spiritCost,

            // learn
            requiredCharacterLevel = requiredCharacterLevel,
            learnCondition = learnCondition,

            // materials
            powerCost = powerCost,
            lthaoCost = lthaoCost,
            mineralCost = mineralCost,
            demonCoreCost = demonCoreCost,
            devilCoreCost = devilCoreCost,
            spiritStoneCost = spiritStoneCost,
            itemCost = itemCost,

            // bonus
            critDamage = critDamage,
            critRate = critRate,
            armorPenetration = armorPenetration,
            trueDamage = trueDamage,
            lifeSteal = lifeSteal,
            attackSpeed = attackSpeed,

            penetrationReduction = penetrationReduction,
            critDamageReduction = critDamageReduction,
            trueDamageReduction = trueDamageReduction,

            bonusHealth = bonusHealth,
            bonusMana = bonusMana,
            bonusSpirit = bonusSpirit,

            totalQualityAndLevel = totalQualityAndLevel,
            statCount = statCount
        };

    }

}