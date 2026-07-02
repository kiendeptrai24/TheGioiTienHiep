


using System;
using System.Collections.Generic;
using UnityEngine;
namespace TGTH.Mobile
{
    public class CharacterCreationPagePresenter : IItemClickHandler
    {
        [Serializable]
        public struct EssneceTypeData
        {
            public string instanceId;
            public EssenceType essenceType;
        }
        [SerializeField] private CharacterCreationPageView view;
        [SerializeField] private IItemDetailPageView itemOnClick;
        [SerializeField] private ActionNavigation navigation;
        [SerializeField] private UIItemSlotBase currentItemSelect;
        [SerializeField] private UIItemSlotBase currentItemCharacter;
        [SerializeField] private string nameCharacter = "";
        private List<ItemData> itemDatas = new List<ItemData>();
        private GameDataCenterManager gameDCM;
        public EssenceType curEssenceType;
        public List<EssneceTypeData> essenceTypes = new List<EssneceTypeData>();
        protected override void Awake()
        {
            base.Awake();
            curEssenceType = EssenceType.Physical;
            LoadComponent();
            view.OnStartClicked += OnStartClicked;
            view.OnFieldEndEdit += OnFieldEndEdit;
            view.OnEssenceTypeDropdownChanged += OnRaceDropdownChanged;
            Init();
        }
        protected override void Start()
        {
            gameDCM = GameDataCenterManager.Instance;
            gameDCM.OnLoadGameDataCenterSuccessed += OnGameBaseCharacterReady;
            if (gameDCM.IsReady())
            {
                OnGameBaseCharacterReady(gameDCM.GetDataCenter());
            }
        }

        private void OnDestroy()
        {
            if (gameDCM != null)
            {
                gameDCM.OnLoadGameDataCenterSuccessed -= OnGameBaseCharacterReady;
            }
        }

        private void OnGameBaseCharacterReady(GameDataCenter center)
        {
            if (center == null) return;
            OnGameBaseCharacterReady(center.characterDatas);
        }

        private void OnGameBaseCharacterReady(List<HeroData> baseCharacterDatas)
        {
            if (baseCharacterDatas == null || baseCharacterDatas.Count == 0) return;
            itemDatas.Clear();
            foreach (var item in baseCharacterDatas)
            {
                itemDatas.Add(item);
            }
            ShowItem(itemDatas);
        }

        private void OnRaceDropdownChanged(int obj)
        {
            curEssenceType = (EssenceType)1 + obj;
        }

        private void OnFieldEndEdit(string obj)
        {
            nameCharacter = obj;
        }
        private void OnEnable()
        {
            view.HideReasonFail();
        }
        private void OnStartClicked()
        {
            if (currentItemCharacter == null || currentItemSelect == null || nameCharacter == "") return;
            var itemData = currentItemSelect.inventoryItem.data as HeroData;
            itemData.itemName = nameCharacter;
            itemData.essenceType = curEssenceType;
            itemData.essenceId = essenceTypes.Find(x => x.essenceType == curEssenceType).instanceId;
            InventoryItem inventoryItem = new InventoryItem(itemData);

            itemOnClick.HandleItemClicked(inventoryItem);
            navigation.OnClick();
        }
        private void ShowItem(List<ItemData> listItem)
        {
            var itemInventories = new List<InventoryItem>();
            foreach (var item in listItem)
            {
                itemInventories.Add(new InventoryItem(item));
            }
            view.ShowAllItems(itemInventories);
        }
        private void Init()
        {
            foreach (var uiItem in view.listOfUIItems)
            {
                uiItem.OnItemClicked += HandleItemClicked;
            }
        }

        private void HandleItemClicked(UIItemSlotBase uiItem)
        {
            if (uiItem == null) return;
            view.ShowInfo(uiItem);
            ItemClicked(uiItem);
        }
        private void ItemClicked(UIItemSlotBase uiItem)
        {
            int index = view.listOfUIItems.IndexOf(uiItem);
            if (index < 0) return;

            view.SelectUIItem(currentItemSelect, uiItem);

            currentItemSelect = uiItem;
        }

        public override void OnItemClicked(UIItemSlotBase uiItem)
        {
            currentItemCharacter = uiItem;
        }
    }
}
