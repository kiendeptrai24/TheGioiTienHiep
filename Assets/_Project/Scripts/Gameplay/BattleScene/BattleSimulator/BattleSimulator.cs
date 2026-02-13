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
        int index = 0;
        int tawait = 0;

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
            events.Add(new BattleEventInit
            {
                time = t,
                ownerUid = s.units[i].uid,
                team = s.units[i].team,
                cell = s.cell[i],
                type = BattleEventType.Init,
                maxHp = s.units[i].hpMax,
                curtHp = s.units[i].hp
            });
        }
        events.Add(new BattleEvent { time = t, type = BattleEventType.Start, });

        ///return new Result();
        while (t < timeLimit)
        {
            if (++iter > MaxIterations)
            {
                Debug.Log("Max iterations reached");
                return End(DecideWinnerByHp(s.units), t, events, recordEvents);
            }
            if (!HasAlive(s.units, TeamId.Heroes)) return End(TeamId.Enemies, t, events, recordEvents);
            if (!HasAlive(s.units, TeamId.Enemies)) return End(TeamId.Heroes, t, events, recordEvents);
            int a = sched.PickNextActor(s, out float bestT);
            if (a < 0 || bestT == float.MaxValue) break;
            t = bestT;
            if (s.units[a].hp <= 0) continue;
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

        return End(TeamId.Enemies, timeLimit, events, recordEvents);
    }

    static bool TryMove(BattleSimState s, BattleScheduler sched, BattleBoardGrid board,
        int a, int target, float t, int dist, List<BattleEvent> events, bool recordEvents)
    {
        if (sched.nextMove[a] > t) return false;

        int myRange = s.atkRange[a];
        bool moved = false;

        Vector2Int from = s.cell[a];
        Vector2Int step = from;
        int cellTomove = 1;
        if (dist > myRange)
        {
            step = board.ChooseMoveStep(from, s.cell[target], s.units);

            if (step != from && board.TryMove(a, from, step, s.units))
            {
                sched.ApplyCell(s, a, step, board.moveInterval);
                moved = true;
                cellTomove = board.Dist(from, step);
                if (recordEvents)
                    events.Add(new BattleEventMove
                    {
                        time = t,
                        team = s.units[a].team,
                        type = BattleEventType.Move,
                        ownerUid = s.units[a].uid,
                        from = from,
                        to = step
                    });
            }
        }

        sched.ScheduleNextMove(a, t, board.moveInterval * cellTomove);
        return moved;
    }

    static void HandleDeath(BattleSimState s, BattleBoardGrid board, int a, int target, float t, List<BattleEvent> events, bool recordEvents)
    {
        if (s.units[target].hp <= 0)
        {
            board.FreeCell(s.cell[target], target);

            if (recordEvents)
                events.Add(new BattleEventDealth
                {
                    time = t,
                    team = s.units[target].team,
                    ownerUid = s.units[target].uid,
                    type = BattleEventType.Death,
                    attackerUid = s.units[a].uid,
                    targetUid = s.units[target].uid
                });
        }

        if (s.units[a].hp <= 0)
        {
            board.FreeCell(s.cell[a], a);

            if (recordEvents)
                events.Add(new BattleEventDealth
                {
                    time = t,
                    team = s.units[a].team,
                    ownerUid = s.units[a].uid,
                    type = BattleEventType.Death,
                    attackerUid = s.units[target].uid,
                    targetUid = s.units[a].uid
                });
        }
    }

    static bool HasAlive(List<UnitSnapshot> units, TeamId team)
    {
        for (int i = 0; i < units.Count; i++)
            if (units[i].team == team && units[i].hp > 0) return true;
        return false;
    }

    static Result End(TeamId winner, float t, List<BattleEvent> events, bool recordEvents)
    {
        if (recordEvents)
            events.Add(new BattleEvent { time = t, type = BattleEventType.End });

        return new Result { winner = winner, duration = t, events = recordEvents ? events : null };
    }
    static TeamId DecideWinnerByHp(List<UnitSnapshot> units)
    {
        long heroHp = 0, enemyHp = 0;
        for (int i = 0; i < units.Count; i++)
        {
            if (units[i].hp <= 0) continue;
            if (units[i].team == TeamId.Heroes) heroHp += units[i].hp;
            else enemyHp += units[i].hp;
        }
        return heroHp >= enemyHp ? TeamId.Heroes : TeamId.Enemies;
    }
}
