// Assets/_Game/WorldMap/Runtime/Pathfinding/PathResult.cs
using System.Collections.Generic;
using WorldMap.Domain;

namespace WorldMap.Pathfinding
{
    public sealed class PathResult
    {
        public readonly List<GridCoord> nodes = new List<GridCoord>(256);
        public bool success;
    }
}
