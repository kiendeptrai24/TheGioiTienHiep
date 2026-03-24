using System;
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
    private PlayerChoseObject choseObject;
    private EntityOptionManager entityOptionManager;
    [SerializeField] private MineInfoPresenter mineInfoPresenter;
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
        if (choseObject.CheckIsOwner())
        {
            choseObject.UnLink();
            LeaveUI();
        }
    }

    private void LeaveUI()
    {
        entityOptionManager.Hide();
    }
    public void ShowUIOwner()
    {
        leaveButton.gameObject.SetActive(true);
        stopMineButton.gameObject.SetActive(true);
        infoButton.gameObject.SetActive(true);
        mineButton.gameObject.SetActive(false);
    }
    public void ShowUINotOwner()
    {
        leaveButton.gameObject.SetActive(true);
        stopMineButton.gameObject.SetActive(false);
        infoButton.gameObject.SetActive(true);
        mineButton.gameObject.SetActive(true);
    }
    private void ShowUI()
    {
        gameObject.SetActive(true);
        if (choseObject == null) return;
        if (choseObject.CheckIsOwner())
        {
            ShowUIOwner();
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
}
