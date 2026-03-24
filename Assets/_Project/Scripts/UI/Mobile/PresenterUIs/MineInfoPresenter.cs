
using TGTH.Mobile;
using UnityEngine;

public class MineInfoPresenter : TGTHMonoBehaviour
{
    [SerializeField] private MineInfoPageView view;
    [SerializeField] private CloseButton backBtn;
    [SerializeField] private ItemResourseData itemResourseData;
    private float currentProduction = 0;
    private float currentMiningProgress = 0;
    private float lastTimeClick = 0;
    private float interval = 1;
    protected override void Awake()
    {
        view.OnMineClicked += StartMine;
    }

    public void Show(ItemData itemData)
    {
        if (itemData == null)
        {
            return;
        }
        itemResourseData = itemData as ItemResourseData;
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
        if (lastTimeClick + interval < Time.time && PlayerChoseObject.Instance.CheckIsOwner() == false)
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