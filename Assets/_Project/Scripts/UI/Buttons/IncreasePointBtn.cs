using UnityEngine;
using UnityEngine.UI;

public class IncreasePointBtn : TGTHMonoBehaviour
{
    private Button increasePointBtn;
    private StatsData statsManager;
    [SerializeField] private StatType statType;
    protected override void Awake()
    {
        LoadComponent();
        increasePointBtn.onClick.AddListener(OnIncreasePointClicked);
    }

    private void OnIncreasePointClicked()
    {
        var popup = PopupManager.Instance.GetPopup<IncreasePointPopup>();
        var data = new BaseSetupData(StatTypeViName.ToVietnamese(statType));

        popup?.ShowPopup(data,
        onConfirm: (StatsPointPopupData result) =>
        {
            if (result == null) return;
            var stat = statsManager.GetStat(statType);

            if (stat == null) return;
            stat.AddModifier(result.value);
            statsManager.StatChange();
        },
        onCancel: () =>
        {
            // leave game
        });
    }

    protected override void LoadComponent()
    {
        base.LoadComponent();
        statsManager = FindAnyObjectByType<StatsData>();
        increasePointBtn = GetComponent<Button>();
    }
}