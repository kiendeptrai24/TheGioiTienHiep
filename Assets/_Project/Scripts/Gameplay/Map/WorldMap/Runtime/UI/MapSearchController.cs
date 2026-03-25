using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;
using WorldMap.Travel;

namespace WorldMap.UI
{
    public class MapSearchController : TGTHNetworkBehaviour
    {
        public ActorController actorController;
        [SerializeField] private string searchText;
        [SerializeField] private List<Destination> destititions;
        private TeleportService teleport = new TeleportService();
        private ResourceManager resourceManager;
        protected override void Awake()
        {
            base.Awake();
            resourceManager = GetComponent<ResourceManager>();
        }
        protected override void Start()
        {
            base.Start();
            destititions.Clear();
        }
        public void TeleportById(string destinationId)
        {

        }
        [ContextMenu("Teleport")]
        public void Teleport()
        {
            Debug.Log("teleport IsOwner");
            if (actorController == null) return;

            var des = destititions.FirstOrDefault(d => d.id == searchText);
            if (des == null) return;

            actorController.RequestTeleportServerRpc(
                des.spawnPoint.position,
                des.spawnPoint.rotation
            );
        }


        public List<Destination> Search(string text)
        {
            return null;
        }
    }
}
