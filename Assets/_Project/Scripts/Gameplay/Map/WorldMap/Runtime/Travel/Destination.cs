// Assets/_Game/WorldMap/Runtime/Travel/Destination.cs
using UnityEngine;

namespace WorldMap.Travel
{
    public enum ResourceType
    {
        SpiritStone,
        Ore,
        Wood,
        Herb
    }


    [System.Serializable]
    public class Destination
    {
        
        public string id;
        public string displayName;
        public ResourceType type;
        public ItemData itemData;
        public Transform spawnPoint; // where player appears
        public bool unlocked = true; // later: progression
        public string[] keywords;    // search support
    }
}
