

using UnityEngine;
namespace TGTH.Mobile
{
    public class CharacterPagePresenter : TGTHMonoBehaviour
    {
        [SerializeField] private CharacterPageView view;
        [SerializeField] private IItemDetailPageView itemDetailPageView;
        [SerializeField] private IItemDetailPageView realmDetailPageView;
        private InventoryCenterManager inventoryCenterManager;

        [SerializeField] private bool isUpgrading = false;
        private string updateRealmName = "Đang bế quan";
        private string baseRealmName = "Cảnh giới";
        private HeroData heroData;
        [SerializeField] private ActionNavigation navigation;
        [SerializeField] private UpgradeState upgradeState;
        protected override void Awake()
        {
            base.Awake();
            LoadComponent();
            Init();
            OnItemPlayerChanged(inventoryCenterManager.playerCham);
            inventoryCenterManager.OnItemPlayerChanged += OnItemPlayerChanged;
            view.SetRealmBtnName(baseRealmName);
            view.OnRealmButtonClicked += () =>
            {
                if (isUpgrading)
                {
                    if (upgradeState == null)
                        return;
                    var duration = TimeUtils.FormatRemainingTime(upgradeState.endTime);

                    TopNotificationUI.Instance.ShowNotification($"Đang trong quá trình đột phá\n" +
                        $"vui lòng đợi {TextColorUtil.Color(duration, Color.green)} kết quả!");
                }
                else
                {
                    InventoryItem inventoryItem = new InventoryItem(inventoryCenterManager.playerCham);
                    realmDetailPageView?.HandleItemClicked(inventoryItem);
                    navigation.OnClick();
                }
            };
            SegmentRealmManager.Instance.OnRealmUpgrade += (UpgradeState upgradeState) =>
            {
                isUpgrading = true;
                view.SetRealmBtnName(updateRealmName);
                this.upgradeState = upgradeState;
            };
            SegmentRealmManager.Instance.OnRealmUplevelResult += (bool result) =>
            {
                if (heroData != null)
                    view.SetRealmBtnName(baseRealmName);
                isUpgrading = false;
            };
            SegmentRealmManager.Instance.RefreshUpgradeState();
        }
        private void Init()
        {
            foreach (var uiItem in view.uIEquipmentSlots)
            {
                uiItem.OnItemClicked += HandleItemClicked;
            }
            view.Init();
        }

        private void HandleItemClicked(UIItemSlotBase uiItem)
        {
            uiItem.navigation.OnClick();
            itemDetailPageView?.HandleItemClicked(uiItem.inventoryItem);
        }
        private void OnItemPlayerChanged(ItemData data)
        {
            heroData = data as HeroData;
            view.ShowData(data);
        }

        protected override void LoadComponent()
        {
            base.LoadComponent();
            view = GetComponent<CharacterPageView>();
            inventoryCenterManager = InventoryCenterManager.Instance;

        }
    }
}
