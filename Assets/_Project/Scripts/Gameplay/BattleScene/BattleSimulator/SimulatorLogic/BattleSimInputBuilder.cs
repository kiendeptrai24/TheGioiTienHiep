using System.Collections.Generic;
using UnityEngine;

public static class BattleSimInputBuilder
{
    public static BattleSimState Build(List<UnitInput> heroes, List<UnitInput> enemies)
    {
        int capacity = heroes.Count + enemies.Count;

        var units = new List<UnitSnapshot>(capacity);
        var skillsByUnit = new List<List<SkillData>>(capacity);

        var cell = new Vector2Int[capacity];
        var atkRange = new int[capacity];

        int idx = 0;

        void Add(UnitInput input)
        {
            units.Add(input.snap);
            skillsByUnit.Add(input.skills);

            cell[idx] = input.placement.cell;
            atkRange[idx] = Mathf.Max(1, input.placement.attackRange);
            idx++;
        }

        for (int i = 0; i < heroes.Count; i++) Add(heroes[i]);
        for (int i = 0; i < enemies.Count; i++) Add(enemies[i]);
        return new BattleSimState(units, skillsByUnit, cell, atkRange);
    }
}
