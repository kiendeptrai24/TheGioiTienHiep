

    using UnityEngine;
    using System;

    [Serializable]
    public class ItemEquitmentData : ItemData
    {
        [Header("Equipment Type")]
        public EquipmentType equipmentType;
        public int level;
        public QualityType qualityType;
        public RaceType raceType; 
        public ElementType elementType;

        // ============================
        // 1. DAMAGE STATS
        // ============================
        [Header("Damage Stats")]
        public float criticalDamage;        // Sát thương chí mạng (%)
        public float criticalRate;          // Tỷ lệ chí mạng (%)

        public float trueDamage;            // Sát thương chuẩn
        public float armorPenetration;      // Xuyên phòng ngự / xuyên giáp
        public float lifeSteal;             // Hút sinh lực (%)
        public float attackSpeed;           // Tốc độ đánh

        // ============================
        // 2. DEFENSE STATS
        // ============================
        [Header("Defense Stats")]
        public float maxHealth;             // Sinh lực
        public float maxMana;               // Linh lực
        public float maxSpirit;             // Linh thức

        public float healthRegen;           // Hồi sinh lực
        public float manaRegen;             // Hồi linh lực
        public float spiritRegen;           // Hồi linh thức

        public float allyHealthRegen;       // Hồi sinh lực cho đồng minh
        public float allyManaRegen;         // Hồi linh lực cho đồng minh
        public float allySpiritRegen;       // Hồi linh thức cho đồng minh

        public float reduceCritDamage;      // Giảm sát thương chí mạng
        public float reduceArmorPen;        // Giảm xuyên giáp
        public float reduceTrueDamage;      // Giảm sát thương chuẩn

        public float reflectDamage;         // Phản đòn (% gây ngược lại)
        public float moveSpeed;             // Tốc độ di chuyển

        // ============================
        // 3. STATUS EFFECT & IMMUNITY
        // ============================
        [Header("Effect & Immunity")]
        public float immuneAllyDamage;       // Miễn sát thương đồng minh
        public float immuneAllyEffects;      // Miễn hiệu ứng đồng minh
        public float immuneAllFromAllies;    // Miễn mọi thứ từ đồng minh

        public float cleanseAllyEffects;    // Giải trừ hiệu ứng đồng đội (%)

        public float grievousWound;         // Vết thương sâu (giảm hồi máu mục tiêu)
        public float reduceEnemyMana;       // Giảm linh lực đối phương
        public float reduceEnemySpirit;     // Giảm linh thức đối phương

        public float weakenTarget;          // Suy yếu (giảm damage mục tiêu)
        public float paralyzeChance;        // Tỷ lệ tê liệt
        public float rootChance;            // Tỷ lệ vây khốn
        public float stunChance;            // Tỷ lệ choáng
        public float silenceChance;         // Tỷ lệ câm lặng

        public float immuneDamage;           // Miễn sát thương
        public float immuneEffects;          // Miễn hiệu ứng
        public float immuneAll;              // Miễn tất cả

        public float reduceEffectDuration;  // Giảm thời gian dính hiệu ứng (%)
        public float effectResistance;      // Kháng hiệu ứng (%)
    }