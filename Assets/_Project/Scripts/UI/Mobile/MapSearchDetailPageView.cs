namespace TGTH.Mobile
{
    using System;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class MapSearchDetailPageView : TGTHMonoBehaviour
    {
        [SerializeField] private Image modelIcon;
        [SerializeField] private Slider realmSlider;
        [SerializeField] private TextMeshProUGUI realmTxt;
        [SerializeField] private Transform resourceContent;
        [SerializeField] private Button okBtn;
        [SerializeField] private Button cancelBtn;
        [SerializeField] private Button addValueBtn;
        [SerializeField] private Button minusValueBtn;
        public List<UIItemResourceType> choseItemType;

        public List<ItemData> items;
        public List<UIItemResourse> uIItemNearBy;
        [SerializeField] private UIItemResourse uIItemPrefab;
        public event Action<int> OnRealmSliderChanged;
        public event Action OnAddClicked;
        public event Action OnMinusClicked;
        public event Action<UIItemResourse> OnCreateNewItem;

        protected override void Awake()
        {
            base.Awake();

            realmSlider.onValueChanged.AddListener((value) =>
            {
                OnRealmSliderChanged?.Invoke((int)value);
                CultivationStage realm = (CultivationStage)value;
                realmTxt.text = EnumTranslator.ToVietnamese(realm);

            });

            minusValueBtn.onClick.AddListener(() => OnMinusClicked?.Invoke());
            addValueBtn.onClick.AddListener(() => OnAddClicked?.Invoke());
        }

        public void Init()
        {
            foreach (var data in items)
            {
                var item = Instantiate(uIItemPrefab, resourceContent);
                item.SetData(data);
                uIItemNearBy.Add(item);
            }
        }
        protected override void Start()
        {
            base.Start();
        }
        public void SetModeIcon(ItemData itemData)
        {
            modelIcon.sprite = itemData.itemIcon;
        }
        public void SortItemType()
        {
            foreach (var item in uIItemNearBy)
            {
                item.ResetData();
            }
        }
        protected override void LoadComponent()
        {
            base.LoadComponent();
        }
        public void ChangeSlider(int value)
        {
            if (value < 0)
            {
                if (realmSlider.value <= 0) return;
                realmSlider.value += value;
            }
            else
            {
                if (realmSlider.value >= 37) return;
                realmSlider.value += value;
            }
        }
        public void ShowItemsByStage(List<ItemData> filteredItems)
        {
            int i = 0;
            for (; i < filteredItems.Count; i++)
            {
                if (i < uIItemNearBy.Count)
                {
                    uIItemNearBy[i].SetData(filteredItems[i]);
                    uIItemNearBy[i].gameObject.SetActive(true);
                }
                else
                {
                    var item = Instantiate(uIItemPrefab, resourceContent);
                    item.SetData(filteredItems[i]);
                    uIItemNearBy.Add(item);
                    OnCreateNewItem?.Invoke(item);
                }
            }
            for (; i < uIItemNearBy.Count; i++)
            {
                uIItemNearBy[i].gameObject.SetActive(false);
            }
        }
    }
}
