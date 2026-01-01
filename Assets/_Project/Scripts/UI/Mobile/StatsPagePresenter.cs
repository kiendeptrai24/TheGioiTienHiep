

using UnityEngine;
namespace TGTH.Mobile
{
    public class StatsPagePresenter : TGTHMonoBehaviour
    {
        [SerializeField] private StatsManager statsManager;
        [SerializeField] private CharacterIdentity characterIdentity;
        [SerializeField] private StatsPageView view;
        protected override void Awake()
        {
            base.Awake();
            view.SetStatsData(statsManager.stats);
            view.ShowCharactorIdentifyData(characterIdentity);
            statsManager.OnValueChanged += UpdateDataItem;
        }
        private void OnEnable()
        {
            UpdateDataItem();
        }
        public void UpdateDataItem()
        {
            view.SetStatsData(statsManager.stats);
            view.ShowCharactorIdentifyData(characterIdentity);
        }
    }
}