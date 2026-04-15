using UnityEngine;
using UnityEngine.UI;

public class IncreasePointBtn : TGTHMonoBehaviour
{
    private Button increasePointBtn;
    private StatsData statsManager;
    [SerializeField] private StatType statType;
    private ProfileManager profileManager;
    protected override void Awake()
    {
        LoadComponent();
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

            var stat = statsManager.GetStat(statType);

            if (stat == null) return;
            stat.AddModifier(result.value);
            var hero = statsManager.heroData as HeroData;
            if (hero == null)
            {
                Debug.Log("hero is null");
                return;
            }

            AddPoint(hero, profileManager.GetProfile().itemDataPoint, result.value, statType);
            statsManager.SetUpItem(hero);
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
                Debug.Log("healthPoint");
                break;
            case StatType.ManaPoint:
                itemDataPoint.manaPoint += value;
                heroData.manaPoint += value;
                Debug.Log("manaPoint");
                break;
            case StatType.SpiritPoint:
                itemDataPoint.spiritPoint += value;
                heroData.spiritPoint += value;
                Debug.Log("spiritPoint");
                break;
            case StatType.PhicialDamagePoint:
                itemDataPoint.damagePoint += value;
                heroData.physicalDamagePoint += value;
                itemDataPoint.damagePoint += value;
                heroData.magicalDamagePoint += value;
                itemDataPoint.damagePoint += value;
                heroData.spiritDamagePoint += value;
                Debug.Log("damagePoint");
                break;
            case StatType.PhicialDefensePoint:
                itemDataPoint.defensePoint += value;
                heroData.physicalDefensePoint += value;
                itemDataPoint.defensePoint += value;
                heroData.magicalDefensePoint += value;
                itemDataPoint.defensePoint += value;
                heroData.spiritDefensePoint += value;
                Debug.Log("defensePoint");
                break;
            case StatType.MoveSpeedPoint:
                itemDataPoint.moveSpeed += value;
                heroData.moveSpeedPoint += value;
                Debug.Log("moveSpeed");
                break;
            case StatType.SpiritRangePoint:
                itemDataPoint.spititRange += value;
                heroData.spititRangePoint += value;
                Debug.Log("spititRange");
                break;
        }
        profileManager.GetProfile().potentialPoint -= value;
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
        statsManager = FindAnyObjectByType<StatsData>();
        increasePointBtn = GetComponent<Button>();
    }
}