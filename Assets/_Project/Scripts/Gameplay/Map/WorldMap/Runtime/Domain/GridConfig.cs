// Assets/_Game/WorldMap/Runtime/Domain/GridConfig.cs
using UnityEngine;

namespace WorldMap.Domain
{
    [CreateAssetMenu(menuName = "WorldMap/GridConfig")]
    public class GridConfig : ScriptableObject
    {
        public int width = 1000;
        public int height = 1000;

        public float cellSize = 1f;           // 1 cell = 1m (1 cube)
        public Vector3 origin = Vector3.zero; // world origin of grid (x,z)
        public float raycastStartY = 200f;    // used by baker
        public float raycastDistance = 500f;
        public LayerMask groundMask;
        public LayerMask obstacleMask;

        public float agentRadius = 0.4f;      // for obstacle overlap
        public float agentHeight = 2.0f;
        public float maxSlopeDeg = 45f;       // optional
    }
}
