// Assets/_Game/WorldMap/Runtime/Travel/Destination.cs
using UnityEngine;

namespace WorldMap.Travel
{
    public enum DestinationType { City, Portal, Dungeon, Waypoint }

    [System.Serializable]
    public class Destination
    {
        public string id;
        public string displayName;
        public DestinationType type;

        public Transform spawnPoint; // where player appears
        public bool unlocked = true; // later: progression
        public string[] keywords;    // search support
    }
}
