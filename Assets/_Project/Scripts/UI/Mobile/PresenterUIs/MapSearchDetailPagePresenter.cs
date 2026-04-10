using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static PathTest;

namespace TGTH.Mobile
{
    public class MapSearchDetailPagePresenter : TGTHMonoBehaviour
    {
        [SerializeField] private MapSearchDetailPageView view;
        [SerializeField] private MapSearchResultPagePresenter presenter;
        [SerializeField] private ResourceManager resource;
        private ResourceType resourceType = ResourceType.LinhThach;
        private RealmType cultivationStage;
        [SerializeField] private PathTest pathTest;
        public NavigationFindMapResult navigationFindMapResult;
        public UIItemResourse curItem;
        private UIItemResourceType curFocusItem;
        private List<FindPathResult> findPathResults = new List<FindPathResult>();
        private int xPos = 0;
        private int yPos = 0;
        private PlayerNetManager playerNet;
        protected override void Awake()
        {
            base.Awake();
            playerNet = PlayerNetManager.Instance;
            view.OnOkClicked += OnOkClicked;
            view.OnRealmSliderChanged += OnRealmSliderChanged;
            view.OnAddClicked += OnAddClicked;
            view.OnMinusClicked += OnMinusClicked;
            view.OnCreateNewItem += OnAddEventItem;
            view.OnXPosChanged += OnXPosChanged;
            view.OnYPosChanged += OnYPosChanged;
            Init();
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
                var resource = pathTest.mapSpawn.WorldToGrid(itemResources.position);
                Debug.Log(itemResources.position);
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
                var result = pathTest.FindPathWithPossition(itemResources.position);

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

                var result = pathTest.FindPathWithPossition(pos);
                if (result.ok)
                {
                    navigationFindMapResult.SetScreenName("MapDetail");
                    pathTest.StartFollowPath();
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
            this.cultivationStage = (RealmType)value;
        }
        public void SetResourceType(ResourceType value)
        {
            this.resourceType = value;
        }
        private void OnRealmSliderChanged(int value)
        {
            SetRealmType(value);
            view.items = resource.GetItemsRange(playerNet.GetPos(), 100);
            var filteredItems = view.items
                .Where(item => item.realmType == this.cultivationStage
                    && item is ItemResourseData resData && resData.resourceType == this.resourceType)
                .ToList();

            view.ShowItemsByStage(filteredItems);
            AddItemEvent();
        }
        private void AddItemNearBy()
        {
            view.items = resource.GetItemsRange(playerNet.GetPos(), 100);
            view.ShowAllItem(view.items);
            AddItemEvent();
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
            curFocusItem?.UnFocusItem();
            curFocusItem = item;
            curFocusItem.FocusItem();
            foreach (var itemUnfocus in view.choseItemType)
            {
                itemUnfocus.UnFocusItem();
            }
            SetResourceType(item.resourceType);
            OnRealmSliderChanged((int)cultivationStage);
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
