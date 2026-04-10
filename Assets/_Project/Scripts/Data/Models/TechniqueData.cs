
using System;
using Newtonsoft.Json;
using UnityEngine;
using static LevelUpValidator;

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
    public int maxEnhanceLevel;          // Cường hóa tối đa  
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
    public int lthaoCost;               // Linh thảo
    [JsonIgnore]
    public int mineralCost;             // Khoáng thạch
    [JsonIgnore]
    public int demonCoreCost;           // Yêu đan
    [JsonIgnore]
    public int devilCoreCost;           // Ma hạch
    [JsonIgnore]
    public int spiritStoneCost;         // Linh thạch
    [JsonIgnore]
    public int itemCost;                // Vật phẩm khác

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
    [JsonIgnore]
    public LevelUpConditionData levelUpConditionData = new();
    public override ItemData Clone()
    {
        var clone = (TechniqueData)this.MemberwiseClone();

        clone.levelUpConditionData = new LevelUpConditionData
        {
            level = this.enhanceLevel,
            conditionType = LevelUpConditionType.TechniqueLevel,
            levelName = this.itemName,
            linhThao = this.lthaoCost,
            khoangThach = this.mineralCost,
            yeuDan = this.demonCoreCost,
            maHach = this.devilCoreCost,
            linhThach = this.spiritStoneCost,
            requiredCharacterLevel = this.requiredCharacterLevel
        };

        return clone;
    }
}