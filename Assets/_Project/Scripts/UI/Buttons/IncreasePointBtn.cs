using ExitGames.Client.Photon.StructWrapping;
using TGTH.Mobile;
using UnityEngine;
using UnityEngine.UI;

public class IncreasePointBtn : TGTHMonoBehaviour
{
    private Button increasePointBtn;
    private StatsData statsData;
    private StatManager statsManager;
    [SerializeField] private StatType statType;
    private ProfileManager profileManager;
    protected override void Awake()
    {
        increasePointBtn = GetComponent<Button>();
        statsManager = StatManager.Instance;
        statsData = statsManager.GetComponent<StatsData>();
        increasePointBtn.onClick.AddListener(OnIncreasePointClicked);
        profileManager = ProfileManager.Instance;
    }

    private void OnIncreasePointClicked()
    {
        var popup = PopupManager.Instance.GetPopup<IncreasePointPopup>();
        var data = new PointSetupData(StatTypeViName.ToVietnamese(statType), "0123456789", 3, profileManager.GetProfile().potentialPoint);

        popup?.ShowPopup(data,
        onConfirm: (StatsPointPopupData result) =>
        {
            if (result == null) return;
            if (result.value > profileManager.GetProfile().potentialPoint)
            {
                TopNotificationUI.Instance.ShowNotification("Số điểm hiện tại của bạn không đủ!");
                return;
            }

            var stat = statsData.GetStatType(statType);

            if (stat == null) return;
            stat.AddModifier(result.value);
            var hero = statsData.heroData as HeroData;
            if (hero == null)
            {
                Debug.Log("hero is null");
                return;
            }

            AddPoint(hero, profileManager.GetProfile().itemDataPoint, result.value, statType);
            statsData.SetUpItem(hero);
            InventoryCenterManager.Instance.ItemPlayerChanged(hero);
            TopNotificationUI.Instance.ShowNotification($"bạn đã cộng {result.value} điểm vào {StatTypeViName.ToVietnamese(statType)}");
        },
        onCancel: () =>
        {
            // leave game
        });
    }
    public void AddPoint(HeroData heroData, ItemDataPoint itemDataPoint, int value, StatType type)
    {
        switch (type)
        {
            case StatType.HealthPoint:
                itemDataPoint.healthPoint += value;
                heroData.healthPoint += value;
                break;
            case StatType.ManaPoint:
                itemDataPoint.manaPoint += value;
                heroData.manaPoint += value;
                break;
            case StatType.SpiritPoint:
                itemDataPoint.spiritPoint += value;
                heroData.spiritPoint += value;
                break;
            case StatType.PhicialDamagePoint:
                itemDataPoint.damagePoint += value;
                heroData.physicalDamagePoint += value;
                heroData.magicalDamagePoint += value;
                heroData.spiritDamagePoint += value;
                break;
            case StatType.PhicialDefensePoint:
                itemDataPoint.defensePoint += value;
                heroData.physicalDefensePoint += value;
                heroData.magicalDefensePoint += value;
                heroData.spiritDefensePoint += value;
                break;
            case StatType.MoveSpeedPoint:
                itemDataPoint.moveSpeedPoint += value;
                heroData.moveSpeedPoint += value;
                break;
            case StatType.SpiritRangePoint:
                itemDataPoint.spititRangePoint += value;
                heroData.spititRangePoint += value;
                break;
        }
        profileManager.GetProfile().potentialPoint -= value;
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
    }
}