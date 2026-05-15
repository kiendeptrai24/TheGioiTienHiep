using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using WorldMap.Domain;
using static PathFinding;

namespace TGTH.Mobile
{
    public class MapSearchDetailPagePresenter : TGTHMonoBehaviour
    {
        [SerializeField] private MapSearchDetailPageView view;
        [SerializeField] private MapSearchResultPagePresenter presenter;
        [SerializeField] private ResourceManager resourceManager;
        private ResourceSourceType resourceSourceType = ResourceSourceType.None;
        private RealmType realmType = RealmType.LuyenKhi_1;
        public NavigationFindMapResult navigationFindMapResult;
        private PathFinding pathFinding;
        public UIItemResourse curItem;
        private UIItemResourceType curFocusItem;
        private List<FindPathResult> findPathResults = new List<FindPathResult>();
        private PlayerNetManager playerNetManager;
        private StatsData statsData;
        private int xPos = 0;
        private int yPos = 0;
        protected override void Awake()
        {
            base.Awake();
            resourceManager = ResourceManager.Instance;
            playerNetManager = PlayerNetManager.Instance;
            pathFinding = PathFinding.Instance;

            playerNetManager.OnPlayerExiststed += OnPlayerExiststed;

            view.OnOkClicked += OnOkClicked;
            view.OnRealmSliderChanged += OnRealmSliderChanged;
            view.OnAddClicked += OnAddClicked;
            view.OnMinusClicked += OnMinusClicked;
            view.OnCreateNewItem += OnAddEventItem;
            view.OnXPosChanged += OnXPosChanged;
            view.OnYPosChanged += OnYPosChanged;

            OnPlayerExiststed(playerNetManager.GetPlayerObj());
            Init();
        }

        private void OnPlayerExiststed(NetworkObject playerNet)
        {
            statsData = playerNet.GetComponent<StatsData>();
        }

        void OnEnable()
        {
            AddItemNearBy();
        }
        private void OnDisable()
        {

            curItem?.UnSelect();
            curItem = null;
            curFocusItem?.UnFocusItem();
            curFocusItem = null;
        }
        private void OnYPosChanged(int value)
        {
            yPos = value;
        }

        private void OnXPosChanged(int value)
        {
            xPos = value;
        }

        private void OnOkClicked()
        {

            if (curItem != null && curItem.itemData != null)
            {
                var itemResources = curItem.itemData as ItemResourseData;
                int xPos = Mathf.RoundToInt(itemResources.position.x);
                int yPos = Mathf.RoundToInt(itemResources.position.z);
                var resource = new GridCoord(xPos, yPos);
                foreach (var item in findPathResults)
                {
                    if (item.goal.x == resource.x && item.goal.z == resource.z)
                    {
                        int index = findPathResults.IndexOf(item);
                        presenter.ShowData(findPathResults, index);

                        navigationFindMapResult.SetScreenName("SearchMapResult");
                        navigationFindMapResult.OnClick();
                        return;
                    }
                }
                var result = pathFinding.FindPathWithPossition(itemResources.position);
                if (result == null) return;
                if (result.ok)
                {
                    result.itemData = itemResources;
                    findPathResults.Add(result);
                    int index = findPathResults.IndexOf(result);
                    presenter.ShowData(findPathResults, index);

                    navigationFindMapResult.SetScreenName("SearchMapResult");
                    navigationFindMapResult.OnClick();
                }
                return;
            }
            else
            {
                Vector3 pos = new Vector3(xPos, 0, yPos);

                var result = pathFinding.FindPathWithPossition(pos);
                if (result.ok)
                {
                    navigationFindMapResult.SetScreenName("MapDetail");
                    pathFinding.StartFollowPath();
                    navigationFindMapResult.OnClick();
                }
                else
                {
                    Debug.Log("Không tìm thấy đường");
                }

            }

        }

        private void OnAddEventItem(UIItemResourse item)
        {
            item.OnItemClicked += OnItemClicked;
        }

        private void OnMinusClicked()
        {
            view.ChangeSlider(-1);
        }

        private void OnAddClicked()
        {
            view.ChangeSlider(1);
        }
        public void SetRealmType(int value)
        {
            this.realmType = (RealmType)value;
        }
        public void SetResourceType(ResourceSourceType value)
        {
            this.resourceSourceType = value;
        }
        private void OnRealmSliderChanged(int value)
        {
            SetRealmType(value);
            if (resourceManager == null || statsData == null || playerNetManager == null) return;

            view.items = resourceManager.GetItemsRange(playerNetManager.GetPos(), statsData.SpiritRange);

            if (view.items == null) return;

            var filteredItems = view.items
                .Where(item => item != null && item.realmType == this.realmType
                    && item is ItemResourseData resData && resData.resourceSourceType == this.resourceSourceType)
                .ToList();

            view.ShowItemsByStage(filteredItems);
            AddItemEvent();
        }
        private void AddItemNearBy()
        {
            if (resourceManager == null || playerNetManager == null || statsData == null) return;
            OnRealmSliderChanged((int)realmType);
        }

        private void AddItemEvent()
        {
            foreach (var item in view.uIItemNearBy)
            {
                item.OnItemClicked += OnItemClicked;
            }
        }

        private void Init()
        {
            foreach (var item in view.choseItemType)
            {
                item.OnItemClicked += OnFocusItem;
            }
        }

        private void OnFocusItem(UIItemResourceType item)
        {
            if (item == curFocusItem) return;
            foreach (var itemUnfocus in view.choseItemType)
            {
                itemUnfocus.UnFocusItem();
            }
            curFocusItem = item;
            curFocusItem.FocusItem();
            SetResourceType(item.resourceSourceType);
            OnRealmSliderChanged((int)realmType);
        }

        private void OnItemClicked(UIItemResourse item)
        {
            curItem?.UnSelect();
            curItem = item;
            curItem.Select();
            view.SetModeIcon(item.itemData);
        }

        protected override void LoadComponent()
        {
            base.LoadComponent();
            view = GetComponent<MapSearchDetailPageView>();

        }
    }
}
