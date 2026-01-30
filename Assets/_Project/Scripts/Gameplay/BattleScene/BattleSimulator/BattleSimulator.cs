using System.Collections.Generic;
using UnityEngine;

public static class BattleSimulator
{
    public struct Result
    {
        public TeamId winner;
        public float duration;
        public List<BattleEvent> events;
    }

    public static Result Simulate(List<UnitSnapshot> heroes, List<UnitSnapshot> enemies, uint seed, float timeLimit = 60f)
    {
        var rng = new XorShift32(seed);
        var events = new List<BattleEvent>(512);

        // gộp unit vào 1 list
        var units = new List<UnitSnapshot>(heroes.Count + enemies.Count);
        units.AddRange(heroes);
        units.AddRange(enemies);

        // next action time cho mỗi unit
        float[] next = new float[units.Count];
        for (int i = 0; i < units.Count; i++)
        {
            // interval = 1 / (attackSpeed) (bạn có thể scale theo design)
            float spd = Mathf.Max(1, units[i].attackSpeed);
            next[i] = 0.2f + (1f / spd);
        }

        float t = 0f;
        while (t < timeLimit)
        {
            // check alive teams
            if (!HasAlive(units, TeamId.Heroes)) return End(TeamId.Enemies, t, events);
            if (!HasAlive(units, TeamId.Enemies)) return End(TeamId.Heroes, t, events);

            // lấy unit có next time nhỏ nhất
            int a = -1;
            float best = float.MaxValue;
            for (int i = 0; i < units.Count; i++)
            {
                if (units[i].hp <= 0) continue;
                if (next[i] < best) { best = next[i]; a = i; }
            }

            if (a < 0) break;
            t = best;

            // chọn target: đơn giản chọn enemy còn sống đầu tiên
            int target = FindFirstAlive(units, units[a].team == TeamId.Heroes ? TeamId.Enemies : TeamId.Heroes);
            if (target < 0) continue;

            // gây damage
            var atk = units[a];
            var def = units[target];
            var (dmg, isCrit, ls, rf) = DamageFormula.Calc(in atk, in def, ref rng);

            def.hp = Mathf.Max(0, def.hp - dmg);

            events.Add(new BattleEvent
            {
                t = t,
                type = BattleEventType.Attack,
                attackerUid = atk.uid,
                targetUid = def.uid,
                damage = dmg,
                isCrit = isCrit,
                targetHpAfter = def.hp
            });

            // lifesteal
            if (ls > 0 && atk.hp > 0)
                atk.hp = Mathf.Min(atk.hpMax, atk.hp + Mathf.RoundToInt(ls));

            // reflect
            if (rf > 0 && def.hp > 0)
                atk.hp = Mathf.Max(0, atk.hp - Mathf.RoundToInt(rf));

            // write-back
            units[a] = atk;
            units[target] = def;

            // death events
            if (def.hp <= 0)
                events.Add(new BattleEvent { t = t, type = BattleEventType.Death, attackerUid = atk.uid, targetUid = def.uid });

            if (atk.hp <= 0)
                events.Add(new BattleEvent { t = t, type = BattleEventType.Death, attackerUid = def.uid, targetUid = atk.uid });

            // schedule next attack
            float spd2 = Mathf.Max(1, units[a].attackSpeed);
            next[a] = t + (1f / spd2);
        }

        // hết giờ: ai còn sống nhiều hơn thắng (bạn chọn “giết hết” nên coi như team không giết được -> thua)
        return End(TeamId.Enemies, timeLimit, events);
    }

    static bool HasAlive(List<UnitSnapshot> units, TeamId team)
    {
        for (int i = 0; i < units.Count; i++)
            if (units[i].team == team && units[i].hp > 0) return true;
        return false;
    }

    static int FindFirstAlive(List<UnitSnapshot> units, TeamId team)
    {
        for (int i = 0; i < units.Count; i++)
            if (units[i].team == team && units[i].hp > 0) return i;
        return -1;
    }

    static Result End(TeamId winner, float t, List<BattleEvent> events)
    {
        events.Add(new BattleEvent { t = t, type = BattleEventType.End, attackerUid = -1, targetUid = -1 });
        return new Result { winner = winner, duration = t, events = events };
    }
}
