namespace TGTH.Mobile
{
    using System;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using static PathTest;

    public class MapSearchResultPageView : TGTHMonoBehaviour
    {
        [SerializeField] private Image modelIcon;
        [SerializeField] private TextMeshProUGUI posResultTxt;
        [SerializeField] private TextMeshProUGUI distanceResultTxt;
        [SerializeField] private Button okBtn;
        [SerializeField] private Button cancelBtn;
        public event Action OnOkClicked;
        public event Action OnCancelClicked;
        protected override void Awake()
        {
            base.Awake();

            okBtn.onClick.AddListener(() => OnOkClicked?.Invoke());
            cancelBtn.onClick.AddListener(() => OnCancelClicked?.Invoke());

        }
        protected override void Start()
        {
            base.Start();
        }
        protected override void LoadComponent()
        {
            base.LoadComponent();
        }
        public void ShowData(FindPathResult result, ItemData itemData)
        {
            if (result == null) return;
            modelIcon.sprite = itemData.itemIcon;
            posResultTxt.text = result.goal.x.ToString() + ":" + result.goal.z.ToString();
            distanceResultTxt.text = result.distance.ToString() + " ô";

        }

    }
}
