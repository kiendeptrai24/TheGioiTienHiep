using System.Collections.Generic;
using UnityEngine;

public static class BattleSimulator
{
    const int MaxIterations = 20_000; // tuỳ máy, có thể 50k/100k/200k

    public struct Result
    {
        public TeamId winner;
        public float duration;
        public List<BattleEvent> events;
    }

    public static Result Simulate(
        List<UnitInput> heroes,
        List<UnitInput> enemies,
        uint seed,
        BattleBoardGrid board,
        float timeLimit = 60f,
        bool recordEvents = true)
    {
        int iter = 0;

        var rng = new XorShift32(seed);
        var events = recordEvents ? new List<BattleEvent>(1024) : null;

        // build state
        BattleSimState s = BattleSimInputBuilder.Build(heroes, enemies);

        // board placement
        board.PlaceAll(s);

        // timers
        var sched = new BattleScheduler(s.units.Count);
        sched.Init(s);

        float t = 0f;
        for (int i = 0; i < s.units.Count; i++)
        {
            List<string> skillIds = new();
            foreach (var skill in s.skillsByUnit[i])
            {
                skillIds.Add(skill.instanceId);
            }
            events.Add(new BattleEventInit
            {
                time = t,
                ownerUid = s.units[i].uid,
                skillIds = skillIds,
                team = s.units[i].team,
                cell = s.cell[i],
                type = BattleEventType.Init,
                maxHp = s.units[i].hpMax,
                curtHp = s.units[i].health,
                maxMana = s.units[i].manaMax,
                curMana = s.units[i].mana,
                maxSpirit = s.units[i].spiritMax,
                curSpirit = s.units[i].spirit,
                moveSpeed = s.units[i].moveSpeed
            });
        }
        events.Add(new BattleEvent { time = t, type = BattleEventType.Start, });

        ///return new Result();
        while (t < timeLimit)
        {
            if (++iter > MaxIterations)
            {
                Debug.Log("Max iterations reached");
                return End(s, DecideWinnerByHp(s.units), t, events, recordEvents);
            }
            if (!HasAlive(s.units, TeamId.Heroes)) return End(s, TeamId.Enemies, t, events, recordEvents);
            if (!HasAlive(s.units, TeamId.Enemies)) return End(s, TeamId.Heroes, t, events, recordEvents);
            int a = sched.PickNextActor(s, out float bestT);
            if (a < 0 || bestT == float.MaxValue) break;
            t = bestT;
            // commit any moves that finished by time t
            sched.CommitPendingMoves(s, board, t);
            if (s.units[a].health <= 0) continue;
            TeamId enemyTeam = (s.units[a].team == TeamId.Heroes) ? TeamId.Enemies : TeamId.Heroes;
            int target = BattleTargeting.FindNearestAlive(s, enemyTeam, a);
            if (target < 0) continue;

            int dist = board.Dist(s.cell[a], s.cell[target]);


            bool acted = false;
            acted =
                BattleCombatResolver.TryCastSkill(s, sched, a, target, t, dist, ref rng, events, recordEvents)
                || BattleCombatResolver.TryBasicAttack(s, sched, a, target, t, dist, ref rng, events, recordEvents) ||
                TryMove(s, sched, board, a, target, t, dist, events, recordEvents);
            // death + free cell + events
            HandleDeath(s, board, a, target, t, events, recordEvents);

            // anti-loop
            if (!acted)
            {
                float wait = Mathf.Min(sched.nextMove[a], sched.nextBasic[a]);
                if (wait == float.MaxValue) break;
                sched.DeferReadySkills(a, t, wait);

            }
        }

        return End(s, TeamId.Enemies, timeLimit, events, recordEvents);
    }

    static bool TryMove(BattleSimState s, BattleScheduler sched, BattleBoardGrid board,
        int a, int target, float t, int dist, List<BattleEvent> events, bool recordEvents)
    {
        if (sched.nextMove[a] > t) return false;

        int myRange = s.atkRange[a];
        bool moved = false;
        float moveSpeed = 1 / s.units[a].moveSpeed;
        Vector2Int from = s.cell[a];
        Vector2Int step = from;

        float cellTomove = 1f;
        if (dist > myRange)
        {
            step = board.ChooseMoveStep(from, s.cell[target], s.units);

            if (step != from && board.TryMove(a, from, step, s.units))
            {
                cellTomove = board.Dist(from, step);
                sched.ApplyCell(s, a, step, t, board.moveInterval * cellTomove * moveSpeed);
                moved = true;
                if (recordEvents)
                    events.Add(new BattleEventMove
                    {
                        time = t,
                        team = s.units[a].team,
                        type = BattleEventType.Move,
                        ownerUid = s.units[a].uid,
                        targetTeam = s.units[target].team,
                        targetUid = s.units[target].uid,
                        from = from,
                        to = step
                    });
                s.units[a].nextActionTime = t + board.moveInterval * cellTomove * moveSpeed;
            }
        }

        sched.ScheduleNextMove(a, t, board.moveInterval * cellTomove * moveSpeed);
        return moved;
    }

    static void HandleDeath(BattleSimState s, BattleBoardGrid board, int a, int target, float t, List<BattleEvent> events, bool recordEvents)
    {
        if (s.units[target].health <= 0)
        {
            board.FreeCell(s.cell[target], target);

            if (recordEvents)
                events.Add(new BattleEventDealth
                {
                    time = t,

                    team = s.units[target].team,
                    targetTeam = s.units[target].team,
                    targetUid = s.units[target].uid,
                    ownerUid = s.units[target].uid,

                    attackerTeam = s.units[a].team,
                    attackerUid = s.units[a].uid,
                    type = BattleEventType.Death,
                });
        }

        if (s.units[a].health <= 0)
        {
            board.FreeCell(s.cell[a], a);

            if (recordEvents)
                events.Add(new BattleEventDealth
                {
                    time = t,

                    team = s.units[a].team,
                    targetTeam = s.units[a].team,
                    targetUid = s.units[a].uid,
                    ownerUid = s.units[a].uid,

                    attackerTeam = s.units[target].team,
                    attackerUid = s.units[target].uid,
                    type = BattleEventType.Death,
                });
        }
    }

    static bool HasAlive(List<UnitSnapshot> units, TeamId team)
    {
        for (int i = 0; i < units.Count; i++)
            if (units[i].team == team && units[i].health > 0) return true;
        return false;
    }

    static Result End(BattleSimState s, TeamId winner, float t, List<BattleEvent> events, bool recordEvents)
    {
        if (recordEvents)
        {
            BattleEventEnd endEvent = new BattleEventEnd();
            endEvent.ownerUid = "";
            endEvent.time = t;
            endEvent.type = BattleEventType.End;
            for (int i = 0; i < s.units.Count; i++)
            {
                if (s.units[i].team == TeamId.Heroes)
                {
                    if (s.units[i].isChacater)
                    {
                        endEvent.heroIsPlayerObject = true;
                        endEvent.maxHealthHero = s.units[i].hpMax;
                        endEvent.maxManaHero = s.units[i].manaMax;
                        endEvent.maxSpiritHero = s.units[i].spiritMax;
                        endEvent.curHealthHero = s.units[i].health;
                        endEvent.curManaHero = s.units[i].mana;
                        endEvent.curSpiritHero = s.units[i].spirit;
                    }
                }
                else if (s.units[i].team == TeamId.Enemies)
                {
                    if (s.units[i].isChacater)
                    {
                        endEvent.enemyIsPlayerObject = true;
                        endEvent.maxHealthEnemy = s.units[i].hpMax;
                        endEvent.maxManaEnemy = s.units[i].manaMax;
                        endEvent.maxSpiritEnemy = s.units[i].spiritMax;
                        endEvent.curHealthEnemy = s.units[i].health;
                        endEvent.curManaEnemy = s.units[i].mana;
                        endEvent.curSpiritEnemy = s.units[i].spirit;
                    }
                }
            }
            events.Add(endEvent);
        }

        return new Result { winner = winner, duration = t, events = recordEvents ? events : null };
    }
    static TeamId DecideWinnerByHp(List<UnitSnapshot> units)
    {
        long heroHp = 0, enemyHp = 0;
        for (int i = 0; i < units.Count; i++)
        {
            if (units[i].health <= 0) continue;
            if (units[i].team == TeamId.Heroes) heroHp += units[i].health;
            else enemyHp += units[i].health;
        }
        return heroHp >= enemyHp ? TeamId.Heroes : TeamId.Enemies;
    }
}
