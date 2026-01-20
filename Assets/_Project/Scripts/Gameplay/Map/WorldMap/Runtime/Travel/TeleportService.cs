// Assets/_Game/WorldMap/Runtime/Travel/TeleportService.cs
using UnityEngine;

namespace WorldMap.Travel
{
    public sealed class TeleportService
    {
        // One responsibility: move player to a destination spawn point
        public bool TryTeleport(Transform player, Destination destination)
        {
            if (player == null || destination == null) return false;
            if (!destination.unlocked) return false;
            if (destination.spawnPoint == null) return false;

            player.position = destination.spawnPoint.position;
            player.rotation = destination.spawnPoint.rotation;
            return true;
        }
    }
}
