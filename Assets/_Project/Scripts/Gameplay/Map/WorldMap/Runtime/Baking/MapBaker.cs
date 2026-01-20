// Assets/_Game/WorldMap/Runtime/Baking/MapBaker.cs
using UnityEngine;
using WorldMap.Domain;
using WorldMap.Data;

namespace WorldMap.Baking
{
    public class MapBaker
    {
        public void BakeInto(MapDataPreset map)
        {
            if (map == null || map.grid == null) return;
            if (map.cells == null || map.cells.Length != map.grid.width * map.grid.height)
                map.Allocate();

            GridConfig cfg = map.grid;

            // One responsibility: fill map.cells by physics queries
            for (int z = 0; z < cfg.height; z++)
                for (int x = 0; x < cfg.width; x++)
                {
                    var cell = new MapCell { walkable = 0, cost = 1, heightY = 0f };

                    // Raycast down to find ground
                    Vector3 origin = GridMath.GridToWorld(cfg, new GridCoord(x, z), cfg.origin.y + cfg.raycastStartY);
                    if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, cfg.raycastDistance, cfg.groundMask))
                    {
                        // Optional slope check
                        float slope = Vector3.Angle(hit.normal, Vector3.up);
                        bool slopeOk = slope <= cfg.maxSlopeDeg;

                        // Obstacle check by overlap capsule/box at that cell
                        Vector3 p = hit.point;
                        float radius = cfg.agentRadius;
                        float height = Mathf.Max(cfg.agentHeight, radius * 2f);

                        Vector3 p1 = p + Vector3.up * radius;
                        Vector3 p2 = p + Vector3.up * (height - radius);

                        bool blocked = Physics.CheckCapsule(p1, p2, radius, cfg.obstacleMask);

                        if (slopeOk && !blocked)
                        {
                            cell.walkable = 1;
                            cell.heightY = hit.point.y;
                            cell.cost = 1; // TODO: set by terrain/material/tag if you want
                        }
                    }

                    map.Set(x, z, cell);
                }
        }
    }
}
