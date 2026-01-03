public readonly struct SkillCastResult
{
    public readonly bool Ok;
    public readonly SkillCastFailReason Reason;
    public readonly string Note;

    public SkillCastResult(bool ok, SkillCastFailReason reason, string note = null)
    {
        Ok = ok;
        Reason = reason;
        Note = note;
    }

    public static SkillCastResult Success() => new(true, SkillCastFailReason.None);
    public static SkillCastResult Fail(SkillCastFailReason r, string note = null) => new(false, r, note);
}
