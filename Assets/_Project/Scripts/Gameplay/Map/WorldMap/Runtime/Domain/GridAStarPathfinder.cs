using System.Collections.Generic;
using UnityEngine;
using WorldMap.Data;
using WorldMap.Domain;

public static class GridAStarPathfinder
{
    private static readonly (int dx, int dz, float cost)[] Neigh8 =
    {
        ( 1, 0, 1f), (-1, 0, 1f), (0, 1, 1f), (0,-1, 1f),
        ( 1, 1, 1.4142f), ( 1,-1, 1.4142f), (-1, 1, 1.4142f), (-1,-1, 1.4142f),
    };

    // 🔥 CACHE (quan trọng)
    private static float[] gScore;
    private static float[] fScore;
    private static int[] cameFrom;
    private static bool[] closed;
    private static bool[] inOpen;

    private static MinHeap open = new MinHeap();

    public static bool TryFindPath(
        MapDataPreset map,
        GridCoord start,
        GridCoord goal,
        List<GridCoord> outPath)
    {
        outPath.Clear();
        if (map == null || map.grid == null || map.cells == null) return false;

        int w = map.grid.width;
        int h = map.grid.height;

        if (!InBounds(start.x, start.z, w, h) || !InBounds(goal.x, goal.z, w, h))
            return false;

        if (map.Get(start.x, start.z).walkable == 0) return false;
        if (map.Get(goal.x, goal.z).walkable == 0) return false;

        int total = w * h;

        // 🔥 INIT / RESIZE (chỉ khi cần)
        EnsureCapacity(total);

        // 🔥 RESET (không tạo GC)
        for (int i = 0; i < total; i++)
        {
            gScore[i] = float.PositiveInfinity;
            fScore[i] = float.PositiveInfinity;
            cameFrom[i] = -1;
            closed[i] = false;
            inOpen[i] = false;
        }

        open.Clear();

        int startIdx = ToIndex(start.x, start.z, w);
        int goalIdx = ToIndex(goal.x, goal.z, w);

        gScore[startIdx] = 0f;
        fScore[startIdx] = Heuristic(start.x, start.z, goal.x, goal.z);

        open.Push(startIdx, fScore[startIdx]);
        inOpen[startIdx] = true;

        while (open.Count > 0)
        {
            int current = open.PopMin();
            if (current == goalIdx)
            {
                ReconstructPath(cameFrom, current, w, outPath);
                return true;
            }

            if (closed[current]) continue;
            closed[current] = true;

            FromIndex(current, w, out int cx, out int cz);

            for (int i = 0; i < Neigh8.Length; i++)
            {
                int nx = cx + Neigh8[i].dx;
                int nz = cz + Neigh8[i].dz;

                if (!InBounds(nx, nz, w, h)) continue;

                int dx = Neigh8[i].dx;
                int dz = Neigh8[i].dz;

                // chống cắt góc
                if (dx != 0 && dz != 0)
                {
                    if (!InBounds(cx + dx, cz, w, h)) continue;
                    if (!InBounds(cx, cz + dz, w, h)) continue;

                    if (map.Get(cx + dx, cz).walkable == 0) continue;
                    if (map.Get(cx, cz + dz).walkable == 0) continue;
                }

                var cell = map.Get(nx, nz);
                if (cell.walkable == 0) continue;

                int nIdx = ToIndex(nx, nz, w);
                if (closed[nIdx]) continue;

                float stepCost = Mathf.Max(1, cell.cost);
                float tentativeG = gScore[current] + stepCost * Neigh8[i].cost;

                if (tentativeG < gScore[nIdx])
                {
                    cameFrom[nIdx] = current;
                    gScore[nIdx] = tentativeG;
                    fScore[nIdx] = tentativeG + Heuristic(nx, nz, goal.x, goal.z);

                    open.Push(nIdx, fScore[nIdx]);
                    inOpen[nIdx] = true;
                }
            }
        }

        return false;
    }

    // 🔥 đảm bảo capacity
    private static void EnsureCapacity(int size)
    {
        if (gScore == null || gScore.Length < size)
        {
            gScore = new float[size];
            fScore = new float[size];
            cameFrom = new int[size];
            closed = new bool[size];
            inOpen = new bool[size];
        }
    }

    private static void ReconstructPath(int[] cameFrom, int current, int w, List<GridCoord> outPath)
    {
        while (current != -1)
        {
            FromIndex(current, w, out int x, out int z);
            outPath.Add(new GridCoord(x, z));
            current = cameFrom[current];
        }
        outPath.Reverse();
    }

    private static float Heuristic(int ax, int az, int bx, int bz)
    {
        int dx = Mathf.Abs(ax - bx);
        int dz = Mathf.Abs(az - bz);
        int min = Mathf.Min(dx, dz);
        int max = Mathf.Max(dx, dz);
        return 1.4142f * min + (max - min);
    }

    private static bool InBounds(int x, int z, int w, int h)
        => x >= 0 && z >= 0 && x < w && z < h;

    private static int ToIndex(int x, int z, int w) => z * w + x;

    private static void FromIndex(int idx, int w, out int x, out int z)
    {
        z = idx / w;
        x = idx - z * w;
    }

    // ===== MinHeap (reuse + clear) =====
    private sealed class MinHeap
    {
        private struct Node
        {
            public int idx;
            public float pri;
            public Node(int i, float p)
            {
                idx = i;
                pri = p;
            }
        }

        private readonly List<Node> a = new List<Node>(1024);

        public int Count => a.Count;

        public void Clear() => a.Clear();

        public void Push(int idx, float pri)
        {
            a.Add(new Node(idx, pri));
            SiftUp(a.Count - 1);
        }

        public int PopMin()
        {
            int result = a[0].idx;
            int last = a.Count - 1;
            a[0] = a[last];
            a.RemoveAt(last);
            if (a.Count > 0) SiftDown(0);
            return result;
        }

        private void SiftUp(int i)
        {
            while (i > 0)
            {
                int p = (i - 1) >> 1;
                if (a[i].pri >= a[p].pri) break;
                (a[i], a[p]) = (a[p], a[i]);
                i = p;
            }
        }

        private void SiftDown(int i)
        {
            int n = a.Count;
            while (true)
            {
                int l = i * 2 + 1;
                int r = l + 1;
                if (l >= n) break;
                int m = (r < n && a[r].pri < a[l].pri) ? r : l;
                if (a[i].pri <= a[m].pri) break;
                (a[i], a[m]) = (a[m], a[i]);
                i = m;
            }
        }
    }
}