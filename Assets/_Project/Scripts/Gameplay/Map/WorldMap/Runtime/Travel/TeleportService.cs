// Assets/_Game/WorldMap/Runtime/Travel/TeleportService.cs
using UnityEngine;

namespace WorldMap.Travel
{
    public sealed class TeleportService
    {
        // One responsibility: move player to a destination spawn point
        public bool TryTeleport(Transform player, Vector3 destination, Quaternion rotation)
        {
            player.position = destination;
            player.rotation = rotation;
            return true;
        }
    }
}
