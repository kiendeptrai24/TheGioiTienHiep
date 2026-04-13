public static class StatPointConfig
{
    public const float DAMAGE_PER_POINT = 2f;
    public const float DEFENSE_PER_POINT = 1.5f;
    public const float HEALTH_PER_POINT = 10f;
    public const float MANA_PER_POINT = 5f;
    public const float SPIRIT_PER_POINT = 3f;
    public const float MOVE_SPEED_PER_POINT = 0.2f;
    public const float SPIRIT_RANGE_PER_POINT = 0.5f;

    public static float GetDamage(int point) => point * DAMAGE_PER_POINT;
    public static float GetDefense(int point) => point * DEFENSE_PER_POINT;
    public static float GetHealth(int point) => point * HEALTH_PER_POINT;
    public static float GetMana(int point) => point * MANA_PER_POINT;
    public static float GetSpirit(int point) => point * SPIRIT_PER_POINT;
    public static float GetMoveSpeed(int point) => point * MOVE_SPEED_PER_POINT;
    public static float GetSpiritRange(int point) => point * SPIRIT_RANGE_PER_POINT;
}