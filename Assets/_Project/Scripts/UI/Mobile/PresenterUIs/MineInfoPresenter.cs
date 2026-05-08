
using TGTH.Mobile;
using UnityEngine;

public class MineInfoPresenter : TGTHMonoBehaviour
{
    [SerializeField] private MineInfoPageView view;
    [SerializeField] private CloseButton backBtn;
    [SerializeField] private SpiritStoneMineData itemResourseData;
    private float currentProduction = 0;
    private float currentMiningProgress = 0;
    private float lastTimeClick = 0;
    private float interval = 1;
    private ProfileManager profileManager;
    protected override void Awake()
    {
        view.OnMineClicked += StartMine;
        profileManager = ProfileManager.Instance;
    }

    public void Show(ItemData itemData)
    {
        if (itemData == null)
        {
            return;
        }
        itemResourseData = itemData as SpiritStoneMineData;
        if (itemResourseData == null)
        {
            return;
        }
        currentProduction = itemResourseData.currentAmount;
        currentMiningProgress = itemResourseData.currentMiningProgress;
        view.Show(itemData);
    }
    void Update()
    {
        if (itemResourseData == null)
            return;
        int currentSecond = Mathf.FloorToInt(itemResourseData.currentMiningProgress);

        if (currentProduction != itemResourseData.currentAmount || currentMiningProgress != currentSecond)
        {
            currentProduction = itemResourseData.currentAmount;
            currentMiningProgress = currentSecond;
            view.UpdateProduction(itemResourseData);
        }
    }
    private void StartMine()
    {
        var playerChose = PlayerChoseObject.Instance;
        if (playerChose.GetCurrentEntity() == null) return;
        var mine = playerChose.GetCurrentEntity().GetComponent<SpiritStoneMine>();
        if (mine == null || mine.PlayerIsOwner(profileManager.GetProfile().userId)) return;

        if (lastTimeClick + interval < Time.time)
        {
            lastTimeClick = Time.time;
            PlayerChoseObject.Instance.RequestBattleSimulator();
            backBtn.OnClick();
        }
        else
        {
            Debug.Log("click too fast");
        }
    }


}