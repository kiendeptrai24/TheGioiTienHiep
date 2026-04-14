namespace TGTH.Mobile
{
    using System;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using static PathFinding;

    public class MapSearchResultPageView : TGTHMonoBehaviour
    {
        [SerializeField] private Image modelIcon;
        [SerializeField] private TextMeshProUGUI modelNameTxt;
        [SerializeField] private TextMeshProUGUI posResultTxt;
        [SerializeField] private TextMeshProUGUI distanceResultTxt;
        [SerializeField] private Button okBtn;
        [SerializeField] private Button cancelBtn;
        [SerializeField] private Button previousBtn;
        [SerializeField] private Button nextBtn;
        public event Action OnOkClicked;
        public event Action OnCancelClicked;
        public event Action OnPreviousClicked;
        public event Action OnNextClicked;
        protected override void Awake()
        {
            base.Awake();
            
            okBtn.onClick.AddListener(() => OnOkClicked?.Invoke());
            cancelBtn.onClick.AddListener(() => OnCancelClicked?.Invoke());
            previousBtn.onClick.AddListener(() => OnPreviousClicked?.Invoke());
            nextBtn.onClick.AddListener(() => OnNextClicked?.Invoke());
        }
        protected override void Start()
        {
            base.Start();
        }
        protected override void LoadComponent()
        {
            base.LoadComponent();
        }
        public void ShowData(FindPathResult result)
        {
            if (result == null) return;
            modelIcon.sprite = result.itemData.itemIcon;
            modelNameTxt.text = result.itemData.itemName;
            posResultTxt.text = result.goal.x.ToString() + ":" + result.goal.z.ToString();
            distanceResultTxt.text = result.distance.ToString() + " ô";

        }

    }
}
