// Assets/_Game/WorldMap/Runtime/Pathfinding/GridPathfinderAStar.cs
using System.Collections.Generic;
using WorldMap.Domain;
using WorldMap.Data;

namespace WorldMap.Pathfinding
{
    public sealed class GridPathfinderAStar
    {
        // One responsibility: find path on MapData grid
        public PathResult FindPath(MapDataPreset map, GridCoord start, GridCoord goal)
        {
            var result = new PathResult();

            if (!GridMath.InBounds(start, map.Width, map.Height) ||
                !GridMath.InBounds(goal, map.Width, map.Height))
                return result;

            // Basic A* (skeleton) - implement fully when you’re ready
            // For now: leave TODO markers so you can fill later.

            // TODO: openSet (priority queue)
            // TODO: cameFrom, gScore
            // TODO: heuristic = Manhattan
            // TODO: reconstruct path

            return result;
        }
    }
}
