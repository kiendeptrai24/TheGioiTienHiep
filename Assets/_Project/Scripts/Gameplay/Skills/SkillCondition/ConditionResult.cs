public readonly struct ConditionResult
{
    public readonly bool Ok;
    public readonly SkillCastFailReason Reason;
    public readonly string DebugNote;

    public ConditionResult(bool ok, SkillCastFailReason reason, string debugNote = null)
    {
        Ok = ok;
        Reason = reason;
        DebugNote = debugNote;
    }

    public static ConditionResult Pass() => new(true, SkillCastFailReason.None);
    public static ConditionResult Fail(SkillCastFailReason reason, string note = null) => new(false, reason, note);
}