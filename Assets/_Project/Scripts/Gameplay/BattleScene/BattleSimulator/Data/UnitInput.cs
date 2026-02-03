using System.Collections.Generic;

public struct UnitInput
{
    public UnitSnapshot snap;
    public List<SkillData> skills; // server-only
    public UnitPlacement placement;
}
