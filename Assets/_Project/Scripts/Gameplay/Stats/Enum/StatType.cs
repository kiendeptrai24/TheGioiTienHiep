public enum StatType
{
    // Tài nguyên
    Health,         // HP
    Mana,           // Mana / Spiritual Power / MP
    Spirit,         // Spirit / Mind Essence

    // Tấn công
    PhysicalDamage, // Sát thương vật lý
    MagicalDamage,  // Sát thương phép / spiritual damage
    SpiritDamage,   // Sát thương tinh thần / mind damage

    // Phòng thủ
    PhysicalDefense,    // Giáp vật lý
    MagicalDefense,     // Kháng phép / spiritual defense
    SpiritDefense,      // Kháng tinh thần / mind defense

    // Xuyên
    SpiritPenetration,  // Xuyên phòng ngự tinh thần
    MindPenetration,    // Nếu bạn muốn tách riêng Mind / Spirit

    // Tốc độ / nhịp combat
    MovementSpeed,
    AttackSpeed,
    CastSpeed,

    // Tiềm năng & điểm
    Potential,      // Điểm tiềm năng / Attribute Points
    SkillPoints,    // Điểm kỹ năng
    CombatPower,    // Lực chiến / Battle Power

    // Nếu vẫn dùng
    CritChance,
    CritPower,
    Evasion
}
