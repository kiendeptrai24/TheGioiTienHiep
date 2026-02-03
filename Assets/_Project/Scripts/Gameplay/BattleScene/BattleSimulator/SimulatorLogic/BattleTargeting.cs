using System.Collections.Generic;
using UnityEngine;

public static class BattleTargeting
{
    public static int FindNearestAlive(BattleSimState s, TeamId enemyTeam, int attackerIndex)
    {
        Vector2Int from = s.cell[attackerIndex];

        int best = -1;
        int bestD = int.MaxValue;
        int bestHp = int.MaxValue;

        for (int i = 0; i < s.units.Count; i++)
        {
            if (s.units[i].hp <= 0) continue;
            if (s.units[i].team != enemyTeam) continue;

            Vector2Int to = s.cell[i];
            int d = Mathf.Abs(to.x - from.x) + Mathf.Abs(to.y - from.y);

            if (d < bestD || (d == bestD && s.units[i].hp < bestHp))
            {
                bestD = d;
                bestHp = s.units[i].hp;
                best = i;
            }
        }

        return best;
    }
}
