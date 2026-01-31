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

    public static Result Simulate(List<UnitInput> heroes, List<UnitInput> enemies, uint seed, float timeLimit = 60f)
    {
        var rng = new XorShift32(seed);
        var events = new List<BattleEvent>(1024);

        // ===== Build Units + Skills =====
        var units = new List<UnitSnapshot>(heroes.Count + enemies.Count);
        var skillsByUnit = new List<List<SkillData>>(heroes.Count + enemies.Count);

        foreach (var h in heroes)
        {
            units.Add(h.snap);
            skillsByUnit.Add(h.skills); // có thể null
        }
        foreach (var e in enemies)
        {
            units.Add(e.snap);
            skillsByUnit.Add(e.skills);
        }

        Debug.Log(units.Count);
        Debug.Log(skillsByUnit.Count);
        // ===== Timers =====
        float[] nextBasic = new float[units.Count];

        // cooldown per unit per skill (variable length)
        var nextSkill = new List<float[]>(units.Count);

        for (int i = 0; i < units.Count; i++)
        {
            float spd = Mathf.Max(1, units[i].attackSpeed);
            nextBasic[i] = 0.2f + (1f / spd);

            var list = skillsByUnit[i];
            if (list == null || list.Count == 0)
            {
                nextSkill.Add(System.Array.Empty<float>());
            }
            else
            {
                var arr = new float[list.Count];
                // skill dùng được ngay => 0
                for (int k = 0; k < arr.Length; k++) arr[k] = 0f;
                nextSkill.Add(arr);
            }
        }

        float t = 0f;

        while (t < timeLimit)
        {
            if (!HasAlive(units, TeamId.Heroes)) return End(TeamId.Enemies, t, events);
            if (!HasAlive(units, TeamId.Enemies)) return End(TeamId.Heroes, t, events);
            
            // ===== pick next actor by nextBasic =====
            int a = -1;
            float best = float.MaxValue;
            for (int i = 0; i < units.Count; i++)
            {
                if (units[i].hp <= 0) continue;
                if (nextBasic[i] < best) { best = nextBasic[i]; a = i; }
            }
            if (a < 0) break;

            t = best;

            int target = FindFirstAlive(units, units[a].team == TeamId.Heroes ? TeamId.Enemies : TeamId.Heroes);
            if (target < 0) continue;

            var atk = units[a];
            var def = units[target];

            // ===== choose ready skill (first ready in list) =====
            int skillIndex = GetReadySkillIndex(skillsByUnit[a], nextSkill[a], t);
            SkillData skill = (skillIndex >= 0) ? skillsByUnit[a][skillIndex] : null;

            int dmg; bool isCrit; float ls; float rf;

            if (skill != null)
            {
                (dmg, isCrit, ls, rf) = DamageFormula.CalcWithSkill(in atk, in def, skill, ref rng);

                // put skill on cooldown
                nextSkill[a][skillIndex] = t + Mathf.Max(0.1f, skill.cooldown);

                events.Add(new BattleEvent
                {
                    t = t,
                    type = BattleEventType.Skill,
                    attackerUid = atk.uid,
                    targetUid = def.uid,
                    damage = dmg,
                    isCrit = isCrit,
                    targetHpAfter = Mathf.Max(0, def.hp - dmg),
                    skillType = (int)skill.skillType,
                    skillIndex = skillIndex
                });
            }
            else
            {
                (dmg, isCrit, ls, rf) = DamageFormula.Calc(in atk, in def, ref rng);

                events.Add(new BattleEvent
                {
                    t = t,
                    type = BattleEventType.Attack,
                    attackerUid = atk.uid,
                    targetUid = def.uid,
                    damage = dmg,
                    isCrit = isCrit,
                    targetHpAfter = Mathf.Max(0, def.hp - dmg),
                    skillType = -1,
                    skillIndex = -1
                });
            }

            // ===== apply dmg =====
            def.hp = Mathf.Max(0, def.hp - dmg);

            // lifesteal / reflect
            if (ls > 0 && atk.hp > 0) atk.hp = Mathf.Min(atk.hpMax, atk.hp + Mathf.RoundToInt(ls));
            if (rf > 0 && def.hp > 0) atk.hp = Mathf.Max(0, atk.hp - Mathf.RoundToInt(rf));

            // write-back
            units[a] = atk;
            units[target] = def;

            // death events
            if (def.hp <= 0)
                events.Add(new BattleEvent { t = t, type = BattleEventType.Death, attackerUid = atk.uid, targetUid = def.uid, skillType = -1, skillIndex = -1 });

            if (atk.hp <= 0)
                events.Add(new BattleEvent { t = t, type = BattleEventType.Death, attackerUid = def.uid, targetUid = atk.uid, skillType = -1, skillIndex = -1 });

            // schedule next basic
            float spd2 = Mathf.Max(1, units[a].attackSpeed);
            nextBasic[a] = t + (1f / spd2);
        }

        return End(TeamId.Enemies, timeLimit, events);
    }

    static int GetReadySkillIndex(List<SkillData> skills, float[] nextSkillTimes, float t)
    {
        if (skills == null || nextSkillTimes == null) return -1;
        int n = Mathf.Min(skills.Count, nextSkillTimes.Length);

        for (int i = 0; i < n; i++)
        {
            var s = skills[i];
            if (s == null) continue;
            // if (!s.hasLearned) continue;          // nếu bạn muốn bắt buộc learned
            if (t >= nextSkillTimes[i]) return i; // ready
        }
        return -1;
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
        events.Add(new BattleEvent { t = t, type = BattleEventType.End, attackerUid = -1, targetUid = -1, skillType = -1, skillIndex = -1 });
        return new Result { winner = winner, duration = t, events = events };
    }
}
