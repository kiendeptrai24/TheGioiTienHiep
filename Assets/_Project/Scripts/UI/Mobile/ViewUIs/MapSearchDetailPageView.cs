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
        [SerializeField] private TextMeshProUGUI modelNameTxt;
        [SerializeField] private Slider realmSlider;
        [SerializeField] private TextMeshProUGUI realmTxt;
        [SerializeField] private Transform resourceContent;
        [SerializeField] private TMP_InputField xPosTxt;
        [SerializeField] private TMP_InputField yPosTxt;
        [SerializeField] private Button okBtn;
        [SerializeField] private Button cancelBtn;
        [SerializeField] private Button addValueBtn;
        [SerializeField] private Button minusValueBtn;
        public List<UIItemResourceType> choseItemType;

        public List<ItemData> items;
        public List<UIItemResourse> uIItemNearBy;
        [SerializeField] private UIItemResourse uIItemPrefab;
        public event Action<int> OnRealmSliderChanged;
        public event Action OnOkClicked;
        public event Action OnAddClicked;
        public event Action OnMinusClicked;
        public event Action<UIItemResourse> OnCreateNewItem;
        public event Action<int> OnYPosChanged;
        public event Action<int> OnXPosChanged;

        protected override void Awake()
        {
            base.Awake();

            realmSlider.onValueChanged.AddListener((value) =>
            {
                OnRealmSliderChanged?.Invoke((int)value);
                RealmType realm = (RealmType)value;
                realmTxt.text = EnumTranslator.ToVietnamese(realm);

            });
            okBtn.onClick.AddListener(() => OnOkClicked?.Invoke());
            minusValueBtn.onClick.AddListener(() => OnMinusClicked?.Invoke());
            addValueBtn.onClick.AddListener(() => OnAddClicked?.Invoke());
            xPosTxt.onValueChanged.AddListener((value) =>
            {
                try
                {
                    int result = int.Parse(xPosTxt.text);
                    OnXPosChanged?.Invoke(result);
                }
                catch (System.Exception)
                {

                }

            });
            yPosTxt.onValueChanged.AddListener((value) =>
            {
                try
                {
                    int result = int.Parse(xPosTxt.text);
                    OnYPosChanged?.Invoke(result);
                }
                catch (System.Exception)
                {

                }

            });
        }

        public void ShowAllItem(List<ItemData> items)
        {
            ClearAllSlots();
            foreach (var data in items)
            {
                var item = Instantiate(uIItemPrefab, resourceContent);
                item.SetData(data);
                uIItemNearBy.Add(item);
            }
        }
        private void ClearAllSlots()
        {
            foreach (var item in uIItemNearBy)
            {
                if (item != null)
                    Destroy(item.gameObject);
            }
            uIItemNearBy.Clear();
        }
        protected override void Start()
        {
            base.Start();
        }
        public void SetModeIcon(ItemData itemData)
        {
            modelIcon.sprite = itemData.itemIcon;
            modelNameTxt.text = itemData.itemName;
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
            ClearAllSlots();
            ShowAllItem(filteredItems);
        }
    }
}
