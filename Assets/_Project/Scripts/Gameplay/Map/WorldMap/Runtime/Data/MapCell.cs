// Assets/_Game/WorldMap/Runtime/Data/MapCell.cs

using System;
using UnityEngine;

namespace WorldMap.Data
{
    [Serializable]
    public struct MapCell
    {
        public byte walkable; // 0/1
        public byte cost;     // 1..255
        public float heightY; // optional, for placing player on ground
        public Vector3 position;
    }
}
