
using System;
using UnityEngine;

[Serializable]
public class TechniqueData : ItemData
{
    // ============================
    // META INFO
    // ============================
    [Header("Meta")]
    public string techniqueId;
    public string techniqueName;
    public bool hasLearned;
    public TechniqueType techniqueType;
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
}