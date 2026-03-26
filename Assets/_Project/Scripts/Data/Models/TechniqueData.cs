
using System;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class TechniqueData : ItemData
{
    // ============================
    // META INFO
    // ============================
    [Header("Meta")]
    [JsonIgnore]
    public bool hasLearned;
    [JsonIgnore]
    public TechniqueType techniqueType;
    [JsonIgnore]
    public int enhanceLevel;             // Cường hóa
    [JsonIgnore]
    public RaceType raceType;            // Tộc
    [JsonIgnore]
    public EssenceType mainEssence;      // Chủ tu
    [JsonIgnore]
    public RealmType realm;       // Cảnh giới

    // ============================
    // COMBAT BEHAVIOR
    // ============================
    [Header("Combat")]
    [JsonIgnore]
    public float attackRange;            // Tầm đánh
    [JsonIgnore]
    public float cooldown;               // Cooldown
    [JsonIgnore]
    public string specialEffect;         // Hiệu ứng (mô tả / key effect)

    // ============================
    // RESOURCE COST
    // ============================
    [Header("Resource Cost")]
    [JsonIgnore]
    public float healthCost;             // Tiêu hao sinh lực
    [JsonIgnore]
    public float manaCost;               // Tiêu hao linh lực
    [JsonIgnore]
    public float spiritCost;             // Tiêu hao linh thức

    // ============================
    // LEARN CONDITIONS
    // ============================
    [Header("Learn Conditions")]
    [JsonIgnore]
    public int requiredCharacterLevel;   // Cấp nhân vật
    [JsonIgnore]
    public string learnCondition;         // Điều kiện học (text / key)

    // ============================
    // CONSUME MATERIALS
    // ============================
    [Header("Upgrade Materials")]
    [JsonIgnore]
    public float powerCost;               // Power
    [JsonIgnore]
    public float lthaoCost;               // Linh thảo
    [JsonIgnore]
    public float mineralCost;             // Khoáng thạch
    [JsonIgnore]
    public float demonCoreCost;           // Yêu đan
    [JsonIgnore]
    public float devilCoreCost;           // Ma hạch
    [JsonIgnore]
    public float spiritStoneCost;         // Linh thạch
    [JsonIgnore]
    public float itemCost;                // Vật phẩm khác

    // ============================
    // OFFENSIVE STATS BONUS
    // ============================
    [Header("Damage Bonus")]
    [JsonIgnore]
    public float critDamage;              // Sát thương chí mạng
    [JsonIgnore]
    public float critRate;                // Tỷ lệ chí mạng
    [JsonIgnore]
    public float armorPenetration;        // Xuyên phòng ngự
    [JsonIgnore]
    public float trueDamage;              // Sát thương chuẩn
    [JsonIgnore]
    public float lifeSteal;               // Hút sinh lực
    [JsonIgnore]
    public float attackSpeed;             // Tốc độ đánh

    // ============================
    // DEFENSIVE STATS BONUS
    // ============================
    [Header("Defense Bonus")]
    [JsonIgnore]
    public float penetrationReduction;   // Giảm sát thương xuyên phòng ngự
    [JsonIgnore]
    public float critDamageReduction;     // Giảm sát thương chí mạng
    [JsonIgnore]
    public float trueDamageReduction;     // Giảm sát thương chuẩn

    // ============================
    // RESOURCE BONUS
    // ============================
    [Header("Resource Bonus")]
    [JsonIgnore]
    public float bonusHealth;             // Tăng sinh lực
    [JsonIgnore]
    public float bonusMana;               // Tăng linh lực
    [JsonIgnore]
    public float bonusSpirit;             // Tăng linh thức

    // ============================
    // SUMMARY / CALCULATION INFO
    // ============================
    [Header("Summary")]
    [JsonIgnore]
    public float totalQualityAndLevel;    // Tổng (phẩm + cấp)
    [JsonIgnore]
    public int statCount;                 // Số chỉ số kích hoạt
    public override ItemData Clone()
    {
        return (TechniqueData)this.MemberwiseClone();
    }
}