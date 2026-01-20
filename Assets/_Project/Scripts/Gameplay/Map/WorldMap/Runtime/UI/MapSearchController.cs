// Assets/_Game/WorldMap/Runtime/UI/MapSearchController.cs
using UnityEngine;
using WorldMap.Travel;

namespace WorldMap.UI
{
    public class MapSearchController : MonoBehaviour
    {
        [SerializeField] private DestinationDatabase database;
        [SerializeField] private Transform player;

        private readonly TeleportService teleport = new TeleportService();

        // One responsibility: glue UI calls to services (no path logic here)
        public void TeleportById(string destinationId)
        {
            var d = database.FindById(destinationId);
            teleport.TryTeleport(player, d);
        }

        // UI can call this to get results and render list (you implement UI list)
        public System.Collections.Generic.List<Destination> Search(string text)
        {
            var list = new System.Collections.Generic.List<Destination>();
            foreach (var d in database.Search(text))
                list.Add(d);
            return list;
        }
    }
}
