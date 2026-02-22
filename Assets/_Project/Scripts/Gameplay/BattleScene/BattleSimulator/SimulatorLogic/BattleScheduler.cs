using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class BattleScheduler
{
    public readonly float[] nextBasic;
    public readonly float[] nextMove;
    public readonly List<float[]> nextSkill;
    readonly Vector2Int[] pendingCell;
    readonly float[] pendingCellTime;

    public BattleScheduler(int nUnits)
    {
        nextBasic = new float[nUnits];
        nextMove = new float[nUnits];
        nextSkill = new List<float[]>(nUnits);
        pendingCell = new Vector2Int[nUnits];
        pendingCellTime = new float[nUnits];
        for (int i = 0; i < nUnits; i++) pendingCellTime[i] = float.MaxValue;
    }

    public void Init(BattleSimState s)
    {
        nextSkill.Clear();

        for (int i = 0; i < s.units.Count; i++)
        {
            float spd = Mathf.Max(1f, s.units[i].attackSpeed);

            nextBasic[i] = 0.2f + (1f / spd);
            nextMove[i] = .1f;

            var list = s.skillsByUnit[i];
            if (list == null || list.Count == 0)
            {
                nextSkill.Add(Array.Empty<float>());
            }
            else
            {
                var arr = new float[list.Count];
                for (int k = 0; k < arr.Length; k++)
                {
                    arr[k] = s.skillsByUnit[i][k].cooldown;
                }
                nextSkill.Add(arr);
            }
        }
    }
    public int PickNextActor(BattleSimState s, out float bestT)
    {
        int a = -1;
        bestT = float.MaxValue;
        for (int i = 0; i < s.units.Count; i++)
        {
            if (s.units[i].hp <= 0) continue;

            float ti = nextBasic[i];

            if (nextMove[i] < ti)
            {
                ti = nextMove[i];
            }

            var skArr = nextSkill[i];
            for (int k = 0; k < skArr.Length; k++)
            {
                if (skArr[k] < ti)
                {
                    ti = skArr[k];
                }
            }

            if (ti < bestT)
            {
                bestT = ti;
                a = i;
            }
        }


        return a;
    }

    public void ScheduleNextBasic(BattleSimState s, int a, float t)
    {
        float spd = Mathf.Max(1f, s.units[a].attackSpeed);
        nextBasic[a] = t + (1f / spd);
    }
    // Schedule the cell change to occur after moveDuration has elapsed (relative to now)
    public void ApplyCell(BattleSimState s, int a, Vector2Int cell, float now, float moveDuration)
    {
        pendingCell[a] = cell;
        pendingCellTime[a] = now + Mathf.Max(0.01f, moveDuration);
        // prevent acting while moving
        nextMove[a] = pendingCellTime[a];
        if (nextBasic.Length > a) nextBasic[a] = Mathf.Max(nextBasic[a], pendingCellTime[a]);
        var sk = nextSkill[a];
        for (int k = 0; k < sk.Length; k++)
            if (sk[k] < pendingCellTime[a]) sk[k] = pendingCellTime[a];
    }

    // Apply any pending cell moves whose scheduled time has arrived
    public void CommitPendingMoves(BattleSimState s, BattleBoardGrid board, float now)
    {
        for (int i = 0; i < pendingCellTime.Length; i++)
        {
            if (pendingCellTime[i] <= now)
            {
                var old = s.cell[i];
                var nw = pendingCell[i];
                if (nw != old)
                {
                    s.cell[i] = nw;
                    board.FreeCell(old, i);
                }
                pendingCellTime[i] = float.MaxValue;
            }
        }
    }
    public void ScheduleNextMove(int a, float t, float moveInterval)
    {
        nextMove[a] = t + Mathf.Max(0.05f, moveInterval);
    }

    public void PutSkillOnCooldown(int a, int skillIndex, float t, float cooldown)
    {
        nextSkill[a][skillIndex] = t + Mathf.Max(0.1f, cooldown);
    }

    public void DeferReadySkills(int a, float now, float waitTime)
    {
        var skArr = nextSkill[a];
        for (int k = 0; k < skArr.Length; k++)
            if (skArr[k] <= now) skArr[k] = waitTime;
    }
}
