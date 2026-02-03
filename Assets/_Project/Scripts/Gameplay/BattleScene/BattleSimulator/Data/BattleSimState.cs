using System.Collections.Generic;
using UnityEngine;

public sealed class BattleSimState
{
    public readonly List<UnitSnapshot> units;
    public readonly List<List<SkillData>> skillsByUnit;

    public readonly Vector2Int[] cell;
    public readonly int[] atkRange;

    public BattleSimState(
        List<UnitSnapshot> units,
        List<List<SkillData>> skillsByUnit,
        Vector2Int[] cell,
        int[] atkRange)
    {
        this.units = units;
        this.skillsByUnit = skillsByUnit;
        this.cell = cell;
        this.atkRange = atkRange;
    }
}
