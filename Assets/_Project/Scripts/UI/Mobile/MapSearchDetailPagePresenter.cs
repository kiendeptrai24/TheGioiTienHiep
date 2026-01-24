using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TGTH.Mobile
{
    public class MapSearchDetailPagePresenter : TGTHMonoBehaviour
    {
        [SerializeField] private MapSearchDetailPageView view;
        [SerializeField] private MapSearchResultPagePresenter presenter;
        [SerializeField] private ResourceManager resource;
        public UIItemResourse curItem;
        private ResourceType resourceType = ResourceType.SpiritStone;
        private CultivationStage cultivationStage;
        [SerializeField] private PathTest pathTest;
        public NavigationFindMapResult navigationFindMapResult;
        protected override void Awake()
        {
            base.Awake();
            view.OnOkClicked += OnOkClicked;
            view.OnRealmSliderChanged += OnRealmSliderChanged;
            view.OnAddClicked += OnAddClicked;
            view.OnMinusClicked += OnMinusClicked;
            view.OnCreateNewItem += OnAddEventItem;
            Init();
        }

        private void OnOkClicked()
        {
            var itemResources = curItem.itemData as ItemResourseData;
            var result = pathTest.FindPathWithPossition(itemResources.position);
            if (result.ok)
            {
                navigationFindMapResult.OnClick();
                presenter.ShowData(result);
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
            this.cultivationStage = (CultivationStage)value;
        }
        public void SetResourceType(ResourceType value)
        {
            this.resourceType = value;
        }
        private void OnRealmSliderChanged(int value)
        {
            SetRealmType(value);

            var filteredItems = view.items
                .Where(item => item.cultivationStage == this.cultivationStage
                    && item is ItemResourseData resData && resData.resourceType == this.resourceType)
                .ToList();

            view.ShowItemsByStage(filteredItems);
        }
        private void Init()
        {
            view.items = resource.GetItems();
            view.Init();
            foreach (var item in view.uIItemNearBy)
            {
                item.OnItemClicked += OnItemClicked;
            }
            foreach (var item in view.choseItemType)
            {
                item.OnItemClicked += OnFocusItem;
            }
        }

        private void OnFocusItem(UIItemResourceType item)
        {
            foreach (var itemUnfocus in view.choseItemType)
            {
                itemUnfocus.UnFocusItem();
            }
            item.FocusItem();
            SetResourceType(item.resourceType);
            OnRealmSliderChanged((int)cultivationStage);
        }

        private void OnItemClicked(UIItemResourse item)
        {
            Debug.Log("OnItemClicked");
            curItem = item;
            view.SetModeIcon(item.itemData);
        }
        protected override void Start()
        {
            base.Start();
        }
        protected override void LoadComponent()
        {
            base.LoadComponent();
            view = GetComponent<MapSearchDetailPageView>();

        }
    }
}
