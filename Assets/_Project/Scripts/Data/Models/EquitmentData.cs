

using UnityEngine;
using System;
using Newtonsoft.Json;

[Serializable]
public class EquitmentData : ItemData
{
    [Header("Equipment Type")]
    [JsonIgnore] 
    public EquipmentType equipmentType;
    [JsonIgnore] 
    public int level;
    [JsonIgnore] 
    public RaceType raceType;

    // ============================
    // 1. DAMAGE STATS
    // ============================
    [Header("Damage Stats")]
    [JsonIgnore] 
    public float critDamage;        // Sát thương chí mạng (%)
    [JsonIgnore] 
    public float critRate;          // Tỷ lệ chí mạng (%)
    [JsonIgnore] 
    public float trueDamage;            // Sát thương chuẩn
    [JsonIgnore] 
    public float armorPenetration;      // Xuyên phòng ngự / xuyên giáp
    [JsonIgnore] 
    public float lifeSteal;             // Hút sinh lực (%)
    [JsonIgnore] 
    public float attackSpeed;           // Tốc độ đánh 

    // ============================
    // 2. DEFENSE STATS
    // ============================
    [Header("Defense Stats")]
    [JsonIgnore] 
    public float maxHealth;             // Sinh lực
    [JsonIgnore]
    public float maxMana;               // Linh lực
    [JsonIgnore]
    public float maxSpirit;             // Linh thức

    [JsonIgnore]
    public float healthRegen;           // Hồi sinh lực
    [JsonIgnore]
    public float manaRegen;             // Hồi linh lực
    [JsonIgnore]
    public float spiritRegen;           // Hồi linh thức

    [JsonIgnore]
    public float allyHealthRegen;       // Hồi sinh lực cho đồng minh
    [JsonIgnore]
    public float allyManaRegen;         // Hồi linh lực cho đồng minh
    [JsonIgnore]
    public float allySpiritRegen;       // Hồi linh thức cho đồng minh

    [JsonIgnore]
    public float critDamageReduction;      // Giảm sát thương chí mạng
    [JsonIgnore]
    public float armorPenetrationReduction;        // Giảm xuyên giáp
    [JsonIgnore]
    public float trueDamageReduction;      // Giảm sát thương chuẩn
    [JsonIgnore]

    public float reflectDamage;         // Phản đòn (% gây ngược lại)
    [JsonIgnore]
    public float moveSpeed;             // Tốc độ di chuyển

    // ============================
    // 3. STATUS EFFECT & IMMUNITY
    // ============================
    [Header("Effect & Immunity")]
    [JsonIgnore]
    public float immuneAllyDamage;       // Miễn sát thương đồng minh
    [JsonIgnore]
    public float immuneAllyEffects;      // Miễn hiệu ứng đồng minh
    [JsonIgnore]
    public float immuneAllFromAllies;    // Miễn mọi thứ từ đồng minh
    [JsonIgnore]

    public float cleanseAllyEffects;    // Giải trừ hiệu ứng đồng đội (%)
    [JsonIgnore]

    public float grievousWound;         // Vết thương sâu (giảm hồi máu mục tiêu)
    [JsonIgnore]
    public float reduceEnemyMana;       // Giảm linh lực đối phương
    [JsonIgnore]
    public float reduceEnemySpirit;     // Giảm linh thức đối phương
    [JsonIgnore]
    public float weakenTarget;          // Suy yếu (giảm damage mục tiêu)
    [JsonIgnore]
    public float paralyzeChance;        // Tỷ lệ tê liệt
    [JsonIgnore]
    public float rootChance;            // Tỷ lệ vây khốn
    [JsonIgnore]
    public float stunChance;            // Tỷ lệ choáng
    [JsonIgnore]
    public float silenceChance;         // Tỷ lệ câm lặng
    [JsonIgnore]

    public float immuneDamage;           // Miễn sát thương
    [JsonIgnore]
    public float immuneEffects;          // Miễn hiệu ứng
    [JsonIgnore]
    public float immuneAll;              // Miễn tất cả
    [JsonIgnore]

    public float reduceEffectDuration;  // Giảm thời gian dính hiệu ứng (%)
    [JsonIgnore]
    public float effectResistance;      // Kháng hiệu ứng (%)
    public override ItemData Clone()
    {
        return (EquitmentData)this.MemberwiseClone();
    }
}