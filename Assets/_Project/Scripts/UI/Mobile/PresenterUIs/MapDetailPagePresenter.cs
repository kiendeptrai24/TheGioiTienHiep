using System;
using System.Collections.Generic;
using UnityEngine;
namespace TGTH.Mobile
{
    public class MapDetailPagePresenter : TGTHMonoBehaviour
    {
        [SerializeField] private MapDetailPageView view;
        [SerializeField] private MinimapIconClickRaycaster minimapIconClickRaycaster;
        private PathFinding pathFinding;
        private int xPos = 0;
        private int zPos = 0;
        protected override void Awake()
        {
            base.Awake();
            minimapIconClickRaycaster.OnDestinationChanged += OnDestinationChanged;
        }
        private void OnDestinationChanged(Vector3 pos)
        {
            if (pathFinding == null) return;
            var result = pathFinding.FindPathWithPossition(pos);
            xPos = (int)pos.x;
            zPos = (int)pos.z;
            if (result.ok)
            {
                pathFinding.StartFollowPath();
                TopNotificationUI.Instance.ShowNotification($"Đang di chuyển đến vị trí ({xPos}, {zPos})");
            }
            else
            {
                TopNotificationUI.Instance.ShowNotification($"Không tìm thấy đường đến vị trí ({xPos}, {zPos})");
            }
        }

        protected override void Start()
        {
            base.Start();
            pathFinding = PathFinding.Instance;
        }
        public void ShowData(InventoryItem inventoryItem)
        {

        }
        protected override void LoadComponent()
        {
            base.LoadComponent();
            view = GetComponent<MapDetailPageView>();
        }
    }
}
