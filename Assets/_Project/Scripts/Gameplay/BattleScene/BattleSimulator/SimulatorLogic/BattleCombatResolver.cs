using System.Collections.Generic;
using UnityEngine;

public static class BattleCombatResolver
{
    public static bool TryCastSkill(
        BattleSimState s,
        BattleScheduler sched,
        int attackerIndex,
        int targetIndex,
        float t,
        int dist,
        ref XorShift32 rng,
        List<BattleEvent> events,
        bool recordEvents)
    {
        int myRange = s.atkRange[attackerIndex];
        int skillIndex = GetReadySkillIndexInRange(s.skillsByUnit[attackerIndex], sched.nextSkill[attackerIndex], t, dist, myRange);
        if (skillIndex < 0)
        {
            return false;
        }
        if (s.units[attackerIndex].nextActionTime > t)
        {
            sched.ScheduleNextBasic(s, attackerIndex, t);
            return false;
        }
        SkillData skill = s.skillsByUnit[attackerIndex][skillIndex];

        var atk = s.units[attackerIndex];
        var def = s.units[targetIndex];

        int dmg; bool isCrit; float ls; float rf;
        (dmg, isCrit, ls, rf) = DamageFormula.CalcWithSkill(in atk, in def, skill, ref rng);

        sched.PutSkillOnCooldown(attackerIndex, skillIndex, t, skill.cooldown + skill.animationDuration);
        s.units[attackerIndex].nextActionTime = t + skill.animationDuration;


        ApplyDamageAndReturn(ref atk, ref def, dmg, ls, rf);

        s.units[attackerIndex] = atk;
        s.units[targetIndex] = def;

        if (recordEvents)
        {
            events.Add(new BattleEventSkill
            {
                time = t,
                type = BattleEventType.Skill,
                team = atk.team,
                targetTeam = def.team,
                ownerUid = atk.uid,
                attackerUid = atk.uid,
                targetUid = def.uid,
                damage = dmg,
                isCrit = isCrit,
                targetHpAfter = def.hp,
                skillId = skill.itemId,
                castTime = skill.castTime
            });
        }

        return true;
    }

    public static bool TryBasicAttack(
        BattleSimState s,
        BattleScheduler sched,
        int attackerIndex,
        int targetIndex,
        float t,
        int dist,
        ref XorShift32 rng,
        List<BattleEvent> events,
        bool recordEvents)
    {
        if (s.units[attackerIndex].nextActionTime > t)
        {
            sched.ScheduleNextBasic(s, attackerIndex, t);
            return false;
        }
        int myRange = s.atkRange[attackerIndex];
        if (dist <= myRange)
        {
            if (sched.nextBasic[attackerIndex] > t)
            {
                return false;
            }
        }
        else
        {
            sched.ScheduleNextBasic(s, attackerIndex, t);
            return false;
        }
        var atk = s.units[attackerIndex];
        var def = s.units[targetIndex];

        int dmg; bool isCrit; float ls; float rf;
        (dmg, isCrit, ls, rf) = DamageFormula.Calc(in atk, in def, ref rng);

        ApplyDamageAndReturn(ref atk, ref def, dmg, ls, rf);

        s.units[attackerIndex] = atk;
        s.units[targetIndex] = def;

        if (recordEvents)
        {
            events.Add(new BattleEventAttack
            {
                time = t,
                type = BattleEventType.Attack,
                team = atk.team,
                targetTeam = def.team,
                ownerUid = atk.uid,
                attackerUid = atk.uid,
                targetUid = def.uid,
                damage = dmg,
                isCrit = isCrit,
                targetHpAfter = def.hp,
                castTime = atk.castTime
            });
        }
        sched.ScheduleNextBasic(s, attackerIndex, t);
        s.units[attackerIndex].nextActionTime = t + atk.animationDuration;
        return true;
    }

    static void ApplyDamageAndReturn(ref UnitSnapshot atk, ref UnitSnapshot def, int dmg, float ls, float rf)
    {
        def.hp = Mathf.Max(0, def.hp - dmg);

        if (ls > 0 && atk.hp > 0) atk.hp = Mathf.Min(atk.hpMax, atk.hp + Mathf.RoundToInt(ls));
        if (rf > 0 && def.hp > 0) atk.hp = Mathf.Max(0, atk.hp - Mathf.RoundToInt(rf));
    }

    static int GetReadySkillIndexInRange(List<SkillData> skills, float[] nextSkillTimes, float t, int distToTarget, int fallbackRange)
    {
        if (skills == null || nextSkillTimes == null) return -1;
        int n = Mathf.Min(skills.Count, nextSkillTimes.Length);
        for (int i = 0; i < n; i++)
        {
            var s = skills[i];
            if (s == null) continue;
            if (t < nextSkillTimes[i]) continue;

            float range = (fallbackRange > s.attackRange) ? fallbackRange : s.attackRange;
            range = Mathf.Max(1, range);
            if (distToTarget <= range)
            {
                return i;
            }
        }
        return -1;
    }
}
