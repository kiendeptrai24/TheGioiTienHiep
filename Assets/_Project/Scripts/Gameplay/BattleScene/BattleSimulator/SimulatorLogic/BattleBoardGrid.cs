using System.Collections.Generic;
using UnityEngine;

public sealed class BattleBoardGrid
{
    public const int MaxWidth = 5;
    public const int TotalRows = 9;

    public readonly int width;
    public readonly int height;
    public readonly bool allowDiagonal;
    public readonly float moveInterval;

    // occ[x,y] = unitIndex
    // -1 = empty
    // int.MinValue = ô không tồn tại
    private readonly int[,] occ;

    public BattleBoardGrid(float moveInterval, bool allowDiagonal = true)
    {
        this.width = MaxWidth;
        this.height = TotalRows;
        this.allowDiagonal = allowDiagonal;
        this.moveInterval = Mathf.Max(0.01f, moveInterval);

        occ = new int[width, height];
        Clear();
    }

    public void Clear()
    {
        for (int y = 0; y < height; y++)
        {
            int rowWidth = GetRowWidth(y);
            for (int x = 0; x < width; x++)
            {
                occ[x, y] = x < rowWidth ? -1 : int.MinValue;
            }
        }
    }

    // 4,5,4,5,4,5,4,5,4
    public int GetRowWidth(int row)
    {
        if (row < 0 || row >= height) return 0;
        return (row % 2 == 0) ? 4 : 5;
    }

    public bool InBoard(Vector2Int p)
    {
        if ((uint)p.y >= (uint)height) return false;
        if ((uint)p.x >= (uint)width) return false;
        return p.x < GetRowWidth(p.y);
    }

    public bool IsCellEmpty(Vector2Int p)
    {
        return InBoard(p) && occ[p.x, p.y] == -1;
    }

    public int GetUnitIndex(Vector2Int p)
    {
        if (!InBoard(p)) return -1;
        return occ[p.x, p.y];
    }

    public void PlaceAll(BattleSimState s)
    {
        Clear();

        for (int i = 0; i < s.units.Count; i++)
        {
            Vector2Int c = s.cell[i];

            if (!InBoard(c))
            {
                Debug.LogError($"Unit uid={s.units[i].uid} invalid cell {c} (board TFT 4/5).");
                continue;
            }

            if (occ[c.x, c.y] != -1)
            {
                Debug.LogError($"Two units overlap at cell {c}. ExistingIndex={occ[c.x, c.y]}, newIndex={i}");
                continue;
            }

            occ[c.x, c.y] = i;
        }
    }

    public void FreeCell(Vector2Int p, int unitIndex)
    {
        if (!InBoard(p)) return;
        if (occ[p.x, p.y] == unitIndex)
            occ[p.x, p.y] = -1;
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

        if (allowDiagonal)
        {
            TryCandidate(from + new Vector2Int(1, 0), toTarget, units, ref best, ref bestScore);
            TryCandidate(from + new Vector2Int(-1, 0), toTarget, units, ref best, ref bestScore);
            TryCandidate(from + new Vector2Int(0, 1), toTarget, units, ref best, ref bestScore);
            TryCandidate(from + new Vector2Int(0, -1), toTarget, units, ref best, ref bestScore);

            TryCandidate(from + new Vector2Int(1, 1), toTarget, units, ref best, ref bestScore);
            TryCandidate(from + new Vector2Int(1, -1), toTarget, units, ref best, ref bestScore);
            TryCandidate(from + new Vector2Int(-1, 1), toTarget, units, ref best, ref bestScore);
            TryCandidate(from + new Vector2Int(-1, -1), toTarget, units, ref best, ref bestScore);
        }
        else
        {
            TryCandidate(from + new Vector2Int(1, 0), toTarget, units, ref best, ref bestScore);
            TryCandidate(from + new Vector2Int(-1, 0), toTarget, units, ref best, ref bestScore);
            TryCandidate(from + new Vector2Int(0, 1), toTarget, units, ref best, ref bestScore);
            TryCandidate(from + new Vector2Int(0, -1), toTarget, units, ref best, ref bestScore);
        }

        return best;
    }

    private void TryCandidate(
        Vector2Int p,
        Vector2Int toTarget,
        List<UnitSnapshot> units,
        ref Vector2Int best,
        ref int bestScore)
    {
        if (!InBoard(p)) return;

        int idx = occ[p.x, p.y];
        if (idx != -1 && units[idx].hp > 0) return;

        int d = Dist(p, toTarget);
        if (d < bestScore)
        {
            bestScore = d;
            best = p;
        }
    }

    public bool TryMove(int unitIndex, Vector2Int from, Vector2Int to, List<UnitSnapshot> units)
    {
        if (to == from) return false;
        if (!InBoard(from) || !InBoard(to)) return false;
        if (occ[from.x, from.y] != unitIndex) return false;

        int dx = Mathf.Abs(to.x - from.x);
        int dy = Mathf.Abs(to.y - from.y);

        bool validStep = allowDiagonal
            ? (dx <= 1 && dy <= 1 && (dx + dy) > 0)
            : ((dx + dy) == 1);

        if (!validStep) return false;

        int idx = occ[to.x, to.y];
        if (idx != -1 && units[idx].hp > 0) return false;

        occ[from.x, from.y] = -1;
        occ[to.x, to.y] = unitIndex;
        return true;
    }

    public IEnumerable<Vector2Int> GetAllCells()
    {
        for (int y = 0; y < height; y++)
        {
            int rowWidth = GetRowWidth(y);
            for (int x = 0; x < rowWidth; x++)
            {
                yield return new Vector2Int(x, y);
            }
        }
    }

    // =========================================================
    // Chuyển cell logic -> vị trí world/UI kiểu TFT
    // =========================================================
    public Vector3 CellToWorld(Vector2Int cell, float cellWidth, float rowHeight, Vector3 origin)
    {
        float offsetX = (GetRowWidth(cell.y) == 4) ? cellWidth * 0.5f : 0f;
        float x = origin.x + offsetX + cell.x * cellWidth;
        float z = origin.z + cell.y * rowHeight;

        return new Vector3(x, origin.y, z);
    }
    public Vector2Int ClampToValidCell(Vector2Int p)
    {
        p.y = Mathf.Clamp(p.y, 0, height - 1);

        int rowWidth = GetRowWidth(p.y);
        p.x = Mathf.Clamp(p.x, 0, rowWidth - 1);

        return p;
    }
}