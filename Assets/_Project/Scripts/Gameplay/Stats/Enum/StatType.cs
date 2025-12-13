public enum StatType
{
    // --- tài nguyên ---
    Health,
    Mana,
    Spirit,


    // --- sát thương ---
    PhysicalDamage, // Sát thương vật lý
    MagicalDamage,  // Sát thương phép 
    SpiritDamage,   // Sát thương tinh thần

    // --- chí mạng ---
    CritChance, // Tỷ lệ chí mạng
    CritPower, // Sát thương chí mạng

    TrueDamage,       // Sát thương chuẩn
    ArmorPenetration,   // Xuyên phòng ngự
    SpiritPenetration,  // Xuyên phòng ngự tinh thần
    LifeSteal,          // Hút sinh lực

    // --- phòng thủ ---
    PhysicalDefense,    // Giáp vật lý
    MagicalDefense,     // Kháng phép
    SpiritDefense,      // Kháng tinh thần
    ReflectDamage,      // Phản đòn

    // --- Giảm sát thương ---
    CritDamageReduction,         // Giảm sát thương chí mạng
    PenetrationDamageReduction,  // Giảm sát thương xuyên giáp
    TrueDamageReduction,         // Giảm sát thương chuẩn

    // --- Hồi phục ---
    HealthRegen,     // Hồi sinh lực
    ManaRegen,       // Hồi linh lực
    SpiritRegen,     // Hồi linh thức
    
    // --- hồi phục đồng minh ---
    AllyHealthRegen, // hồi phục máu cho đồng minh
    AllyManaRegen, // hồi phục năng lượng cho đồng minh
    AllySpiritRegen, // hồi phục thần thức cho đồng minh
    
    // --- Miễn nhiễm ---
    DamageImmunity,   // Miễn nhiễm sát thương
    CCImmunity,       // Miễn nhiễm hiệu ứng
    FullImmunity,     // Miễn nhiễm tất cả

    // --- Miễn nhiễm cho đồng minh ---
    AllyDamageImmunity,   // Miễn nhiễm Sát thương đồng minh
    AllyCCImmunity,       // Miễn nhiễm hiệu ứng đồng minh
    AllyFullImmunity,     // Miễn nhiễm tất cả đồng minh

    // --- Hỗ trợ đồng minh ---
    AllyCleanse,// Giải trừ hiệu ứng đồng đội

    // --- Debuff gây lên địch ---
    HealingReduction,   // Vết thương sâu (giảm hồi máu)
    EnemyManaReduction,    // Giảm linh lực đối phương
    EnemySpiritReduction,  // Giảm linh thức đối phương

    // --- Hiệu ứng bất lợi (CC / Debuff) ---
    Weaken,     // Suy yếu
    Paralyze,   // Tê liệt
    Root,       // Vây khốn
    Stun,       // Choáng
    Silence,    // Câm lặng


    // --- Kháng và giảm hiệu ứng ---
    CCDurationReduction,  // Giảm thời gian hiệu ứng
    CCResistance,         // Kháng hiệu ứng

    // --- tốc độ nhịp combat ---
    MovementSpeed,
    AttackSpeed,
    CastSpeed,

    // --- Điểm tiền năng & điểm kĩ năng ---
    Potential,      // Điểm tiềm năng 
    SkillPoints,    // Điểm kỹ năng
    CombatPower,    // Lực chiến

    SpiritRange,    // Phạm vi linh thức điểm (Spirit Range / Tầm linh thức)
    
    Evasion, //Né tránh
}
