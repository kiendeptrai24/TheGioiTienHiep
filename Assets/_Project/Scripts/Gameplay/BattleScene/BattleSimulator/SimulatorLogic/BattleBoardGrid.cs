using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class BattleBoardGrid
{
    public readonly int width;
    public readonly int height;
    public readonly bool allowDiagonal;
    public readonly float moveInterval;

    // occ[x,y] = unitIndex, -1 empty
    private readonly int[,] occ;

    public BattleBoardGrid(int width, int height, float moveInterval, bool allowDiagonal)
    {
        this.width = width;
        this.height = height;
        this.allowDiagonal = allowDiagonal;
        this.moveInterval = Mathf.Max(0.01f, moveInterval);

        occ = new int[width, height];
        Clear();
    }

    public void Clear()
    {
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                occ[x, y] = -1;
    }

    public bool InBoard(Vector2Int p) => (uint)p.x < (uint)width && (uint)p.y < (uint)height;

    public void PlaceAll(BattleSimState s)
    {
        Clear();

        for (int i = 0; i < s.units.Count; i++)
        {
            Vector2Int c = s.cell[i];
            if (!InBoard(c))
                Debug.Log($"Unit uid={s.units[i].uid} invalid cell {c} (board {width}x{height})");

            if (occ[c.x, c.y] != -1)
                Debug.Log($"Two units overlap at cell {c}. ExistingIndex={occ[c.x, c.y]}, newIndex={i}");

            occ[c.x, c.y] = i;
        }
    }

    public void FreeCell(Vector2Int p, int unitIndex)
    {
        if (!InBoard(p)) return;
        if (occ[p.x, p.y] == unitIndex) occ[p.x, p.y] = -1;
    }

    public int Dist(Vector2Int a, Vector2Int b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);
        return allowDiagonal ? Mathf.Max(dx, dy) : (dx + dy);
    }

    public Vector2Int ChooseMoveStep(Vector2Int from, Vector2Int toTarget, List<UnitSnapshot> units)
    {
        Vector2Int best = from;
        int bestScore = Dist(from, toTarget);

        Span<Vector2Int> dirs = allowDiagonal
            ? stackalloc Vector2Int[8]
            {
                new Vector2Int( 1, 0),
                new Vector2Int(-1, 0),
                new Vector2Int( 0, 1),
                new Vector2Int( 0,-1),
                new Vector2Int( 1, 1),
                new Vector2Int( 1,-1),
                new Vector2Int(-1, 1),
                new Vector2Int(-1,-1),
            }
            : stackalloc Vector2Int[4]
            {
                new Vector2Int( 1, 0),
                new Vector2Int(-1, 0),
                new Vector2Int( 0, 1),
                new Vector2Int( 0,-1),
            };

        for (int i = 0; i < dirs.Length; i++)
        {
            var p = from + dirs[i];
            if (!InBoard(p)) continue;

            int idx = occ[p.x, p.y];
            if (idx != -1 && units[idx].hp > 0) continue;

            int d = Dist(p, toTarget);
            if (d < bestScore)
            {
                bestScore = d;
                best = p;
            }
        }

        return best;
    }

    public bool TryMove(int unitIndex, Vector2Int from, Vector2Int to, List<UnitSnapshot> units)
    {
        if (to == from) return false;
        if (!InBoard(to)) return false;

        int idx = occ[to.x, to.y];
        if (idx != -1 && units[idx].hp > 0) return false;

        occ[from.x, from.y] = -1;
        occ[to.x, to.y] = unitIndex;
        return true;
    }
}
