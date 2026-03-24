using UnityEngine;
using UnityEngine.UI;

public class MonsterOptionUI : TGTHMonoBehaviour, IEntityOptionUI
{
    [SerializeField] private Button infoButton;
    [SerializeField] private Button attackButton;
    [SerializeField] private Button chatButton;
    [SerializeField] private Button leaveButton;
    [SerializeField] private IEnemyInfo enemyInfo;
    private PlayerChoseObject choseObject;
    private EntityOptionManager entityOptionManager;
    public void SetEntity(PlayerChoseObject entity)
    {
        choseObject = entity;
        ShowUI();
    }

    protected override void Awake()
    {
        base.Awake();
        entityOptionManager = GetComponentInParent<EntityOptionManager>();
        leaveButton.onClick.AddListener(() =>
        {
            LeaveUI();
        });
        chatButton.onClick.AddListener(() =>
        {
            ChatUI();
        });
        infoButton.onClick.AddListener(() =>
        {
            ShowInfo();
        });
        attackButton.onClick.AddListener(() =>
        {
            Attack();
        });
    }

    private void ChatUI()
    {

    }

    private void Attack()
    {
        choseObject.RequestBattleSimulator();
        LeaveUI();
    }

    private void ShowInfo()
    {
        var entity = choseObject.GetCurrentEntity();
        var roster = entity.GetComponent<PlayerBattleRoster>();
        enemyInfo.SetupDataInfo(roster.itemDatas);
    }
    private void ShowUI()
    {
        gameObject.SetActive(true);
        leaveButton.gameObject.SetActive(true);
        chatButton.gameObject.SetActive(true);
        infoButton.gameObject.SetActive(true);
        attackButton.gameObject.SetActive(true);
    }
    private void LeaveUI()
    {
        entityOptionManager.Hide();
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
