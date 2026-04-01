using System;
using ExitGames.Client.Photon.StructWrapping;
using FeatureToggles;
using PlayFab.Internal;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class MineOptionUI : TGTHMonoBehaviour, IEntityOptionUI
{
    [SerializeField] private Button leaveButton;
    [SerializeField] private Button stopMineButton;
    [SerializeField] private Button mineButton;
    [SerializeField] private Button infoButton;
    [SerializeField] private Button enemyInfoBtn;
    private PlayerChoseObject choseObject;
    private EntityOptionManager entityOptionManager;
    [SerializeField] private MineInfoPresenter mineInfoPresenter;
    [SerializeField] private IEnemyInfo enemyInfo;
    private PlayerProfile playerProfile;
    public void SetEntity(PlayerChoseObject entity)
    {
        choseObject = entity;
        playerProfile = choseObject.GetPlayerNet().GetComponent<PlayerProfile>();
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
        stopMineButton.onClick.AddListener(() =>
        {
            StopMine();
        });
        infoButton.onClick.AddListener(() =>
        {
            ShowInfo();
        });
        mineButton.onClick.AddListener(() =>
        {
            Mine();
        });
        enemyInfoBtn.onClick.AddListener(() =>
        {
            ShowEnemyInfo();
        });
    }

    private void ShowEnemyInfo()
    {
        var entity = choseObject.GetCurrentEntity();
        var roster = entity.GetComponent<PlayerBattleRoster>();
        if (roster == null) return;
        roster.GetPlayerTeam(() =>
        {
            enemyInfo.SetupDataInfo(roster.itemDatas);
        });
    }

    void OnEnable()
    {
        ShowUI();
    }
    private void Mine()
    {
        choseObject.RequestBattleSimulator();
        LeaveUI();
    }

    private void ShowInfo()
    {
        var entity = choseObject.GetCurrentEntity();
        var resourse = entity.GetComponent<SpiritStoneMine>();
        mineInfoPresenter.Show(resourse.GetItemResourseData());
    }

    private void StopMine()
    {
        if (choseObject == null) return;
        if (choseObject.GetCurrentEntity() == null) return;
        var mine = choseObject.GetCurrentEntity().GetComponent<SpiritStoneMine>();
        if (mine == null) return;
        if (mine.PlayerIsOwner(playerProfile.GetPlayerId()))
        {
            choseObject.UnLink();
            LeaveUI();
        }
    }

    private void LeaveUI()
    {
        entityOptionManager.Hide();
    }
    #region UI

    public void ShowUIOwner()
    {
        leaveButton.gameObject.SetActive(true);
        stopMineButton.gameObject.SetActive(true);
        infoButton.gameObject.SetActive(true);
        mineButton.gameObject.SetActive(false);
        enemyInfoBtn.gameObject.SetActive(false);
    }
    public void ShowUINotOwner()
    {
        leaveButton.gameObject.SetActive(true);
        stopMineButton.gameObject.SetActive(false);
        infoButton.gameObject.SetActive(true);
        mineButton.gameObject.SetActive(true);
        enemyInfoBtn.gameObject.SetActive(false);
    }
    public void ShowUIOther()
    {
        leaveButton.gameObject.SetActive(true);
        stopMineButton.gameObject.SetActive(false);
        infoButton.gameObject.SetActive(true);
        mineButton.gameObject.SetActive(true);
        enemyInfoBtn.gameObject.SetActive(true);
    }
    private void ShowUI()
    {
        gameObject.SetActive(true);
        if (choseObject == null) return;
        var mineClick = choseObject.GetCurrentEntity();
        if (mineClick == null) return;
        var resourse = mineClick.GetComponent<SpiritStoneMine>();
        if (resourse == null) return;

        if (resourse.HasOwner())
        {
            if (resourse.PlayerIsOwner(playerProfile.GetPlayerId()))
                ShowUIOwner();
            else
                ShowUIOther();
        }
        else
        {
            ShowUINotOwner();
        }
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    #endregion

}
