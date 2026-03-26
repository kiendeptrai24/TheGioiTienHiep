using System;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class SkillData : ItemData
{
    // ============================
    // META INFO
    // ============================
    [Header("Meta")]
    [JsonIgnore]
    public bool hasLearned;
    [JsonIgnore]
    public SkillType skillType;          // tên kỹ năng (enum)
    [JsonIgnore]
    public int enhanceLevel;             // Cường hóa
    [JsonIgnore]
    public RaceType raceType;            // Tộc
    [JsonIgnore]
    public EssenceType mainEssence;      // Chủ tu
    [JsonIgnore]
    public RealmType realm;       // Cảnh giới
    [JsonIgnore]
    public GameObject skillEffectPrefab; // Prefab hiệu ứng kỹ năng

    // ============================
    // COMBAT BEHAVIOR
    // ============================
    [Header("Combat")]
    [JsonIgnore]
    public float attackRange;            // Tầm đánh
    [JsonIgnore]
    public float cooldown;               // Cooldown (giây)
    [JsonIgnore]
    [TextArea] public string specialEffect; // Hiệu ứng / mô tả

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
    public string learnCondition;        // Điều kiện học
    [JsonIgnore]
    public string otherNote;             // Khác

    // ============================
    // UPGRADE MATERIALS
    // ============================
    [Header("Upgrade Materials")]
    [JsonIgnore]
    public float powerCost;              // Power
    [JsonIgnore]
    public float lthaoCost;              // Linh thảo
    [JsonIgnore]
    public float mineralCost;            // Khoáng thạch
    [JsonIgnore]
    public float demonCoreCost;          // Yêu đan
    [JsonIgnore]
    public float devilCoreCost;          // Ma hạch
    [JsonIgnore]
    public float spiritStoneCost;        // Linh thạch
    [JsonIgnore]
    public float itemCost;               // Vật phẩm khác

    // ============================
    // DAMAGE BONUS (%)
    // ============================
    [Header("Damage Bonus")]
    [JsonIgnore]
    public float critDamage;             // Sát thương chí mạng (%)
    [JsonIgnore]
    public float critRate;               // Tỷ lệ chí mạng (%)

    [JsonIgnore]
    public float armorPenetration;       // Xuyên phòng ngự (% hoặc điểm)
    [JsonIgnore]
    public float trueDamage;             // Sát thương chuẩn (% hoặc điểm)
    [JsonIgnore]
    public float lifeSteal;              // Hút sinh lực (%)
    [JsonIgnore]
    public float attackSpeed;            // Tốc độ đánh (% hoặc điểm)

    // ============================
    // DEFENSE BONUS
    // ============================
    [Header("Defense Bonus")]
    [JsonIgnore]
    public float penetrationReduction;   // Giảm sát thương xuyên phòng ngự (%)
    [JsonIgnore]
    public float critDamageReduction;    // Giảm sát thương chí mạng (%)
    [JsonIgnore]
    public float trueDamageReduction;    // Giảm sát thương chuẩn (%)

    // ============================
    // RESOURCE BONUS
    // ============================
    [Header("Resource Bonus")]
    [JsonIgnore]
    public float bonusHealth;            // Tăng sinh lực (%)
    [JsonIgnore]
    public float bonusMana;              // Tăng linh lực (%)
    [JsonIgnore]
    public float bonusSpirit;            // Tăng linh thức (%)

    // ============================
    // SUMMARY / CALCULATION
    // ============================
    [Header("Summary")]
    [JsonIgnore]
    public float totalQualityAndLevel;   // Tổng (phẩm + cấp)
    [JsonIgnore]
    public int statCount;                // Số chỉ số kích hoạt
    [Header("Animation")]
    [JsonIgnore]
    public float animationDuration;       // Thời gian animation (giây)
    [JsonIgnore]
    public float castTime;               // Thời gian cast (giây)
    [JsonIgnore]
    public GameObject networkSkillEffectPrefab;
    public override ItemData Clone()
    {
        return (SkillData)this.MemberwiseClone();
    }
}
