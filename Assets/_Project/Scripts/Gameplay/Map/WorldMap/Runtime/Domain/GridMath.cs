// Assets/_Game/WorldMap/Runtime/Domain/GridMath.cs
using UnityEngine;

namespace WorldMap.Domain
{
    public static class GridMath
    {
        public static int ToIndex(int x, int z, int width) => z * width + x;

        public static bool InBounds(GridCoord c, int width, int height)
            => c.x >= 0 && c.z >= 0 && c.x < width && c.z < height;

        public static Vector3 GridToWorld(GridConfig cfg, GridCoord c, float y = 0f)
        {
            // Center of cell
            float wx = cfg.origin.x + (c.x + 0.5f) * cfg.cellSize;
            float wz = cfg.origin.z + (c.z + 0.5f) * cfg.cellSize;
            return new Vector3(wx, y, wz);
        }

        public static GridCoord WorldToGrid(GridConfig grid, Vector3 world)
        {
            float size = grid.cellSize;
            int x = Mathf.FloorToInt(world.x / size);
            int z = Mathf.FloorToInt(world.z / size);
            return new GridCoord(x, z);
        }
    }
}
