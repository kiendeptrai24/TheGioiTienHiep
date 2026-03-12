

using UnityEngine;
namespace TGTH.Mobile
{
    public class StatsPagePresenter : TGTHMonoBehaviour
    {
        [SerializeField] private StatsData statsManager;
        [SerializeField] private StatsPageView view;
        protected override void Awake()
        {
            base.Awake();
            UpdateDataItem();
            statsManager.OnValueChanged += UpdateDataItem;
        }
        private void OnEnable()
        {
            UpdateDataItem();
        }
        public void UpdateDataItem()
        {
            view.SetStatsData(statsManager.stats);
            view.ShowCharactorIdentifyData(statsManager.heroData);
        }
    }
}