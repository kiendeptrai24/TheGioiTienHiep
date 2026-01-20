// Assets/_Game/WorldMap/Runtime/Data/MapData.cs
using UnityEngine;
using WorldMap.Domain;

namespace WorldMap.Data
{
    [CreateAssetMenu(menuName = "WorldMap/MapData")]
    public class MapDataPreset : ScriptableObject
    {
        public GridConfig grid;
        public MapCell[] cells; // length = width*height

        public int Width => grid.width;
        public int Height => grid.height;

        public void Allocate()
        {
            int len = grid.width * grid.height;
            cells = new MapCell[len];
        }

        public MapCell Get(int x, int z)
        {
            int idx = GridMath.ToIndex(x, z, grid.width);
            return cells[idx];
        }

        public void Set(int x, int z, MapCell cell)
        {
            int idx = GridMath.ToIndex(x, z, grid.width);
            cells[idx] = cell;
        }
        [ContextMenu("Count")]
        private void CountWalkable() => Debug.Log(cells.Length);
    }
}
