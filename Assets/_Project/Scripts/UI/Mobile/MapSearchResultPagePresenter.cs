namespace TGTH.Mobile
{
    using System;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using static PathTest;

    public class MapSearchResultPagePresenter : TGTHMonoBehaviour
    {
        [SerializeField] private MapSearchResultPageView view;
        [SerializeField] private PathTest pathTest;
        private FindPathResult result;
        protected override void Awake()
        {
            base.Awake();
            view.OnOkClicked += OnOkClicked;
            view.OnCancelClicked += OnCancelClicked;
        }

        private void OnOkClicked()
        {
            pathTest.StartFollowPath();
        }

        private void OnCancelClicked()
        {

        }
        public void ShowData(FindPathResult result, ItemData itemData)
        {
            view.ShowData(result, itemData);
        }
        protected override void Start()
        {
            base.Start();
        }
        protected override void LoadComponent()
        {
            base.LoadComponent();
            view = GetComponent<MapSearchResultPageView>();
        }
    }
}
