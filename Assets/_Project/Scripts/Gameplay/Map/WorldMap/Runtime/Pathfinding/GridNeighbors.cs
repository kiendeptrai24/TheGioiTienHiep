// Assets/_Game/WorldMap/Runtime/Pathfinding/GridNeighbors.cs
using System.Collections.Generic;
using WorldMap.Domain;

namespace WorldMap.Pathfinding
{
    public static class GridNeighbors
    {
        // 4-dir (đơn giản, ổn định). Bạn đổi 8-dir sau.
        public static IEnumerable<GridCoord> Get4(GridCoord c)
        {
            yield return new GridCoord(c.x + 1, c.z);
            yield return new GridCoord(c.x - 1, c.z);
            yield return new GridCoord(c.x, c.z + 1);
            yield return new GridCoord(c.x, c.z - 1);
        }
    }
}
