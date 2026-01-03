public enum SkillCastFailReason
{
    None = 0,
    OnCooldown,
    NotEnoughMana,
    NotEnoughStamina,
    NoTarget,
    TargetDead,
    OutOfRange,
    CasterStateBlocked,
    Custom
}