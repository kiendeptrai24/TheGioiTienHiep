using System.Collections.Generic;
using UnityEngine;
namespace TGTH.Mobile
{
    public class MapDetailPagePresenter : TGTHMonoBehaviour
    {
        [SerializeField] private MapDetailPageView view;
        protected override void Awake()
        {
            base.Awake();
        }
        protected override void Start()
        {
            base.Start();
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
