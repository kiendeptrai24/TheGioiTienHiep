


using System;
using System.Collections.Generic;
using TGTH.Mobile;
using UnityEngine;

public class EnemyInfoPresenter : IEnemyInfo
{
    [SerializeField] private EnemyInfoPageView view;
    [SerializeField] private CloseButton backBtn;
    private bool setup = false;
    protected override void Awake()
    {
        if (setup == false)
        {
            Init();
            setup = true;
        }
        view.OnAttackClicked += StartAttack;

    }

    private void StartAttack()
    {
        PlayerChoseObject.Instance.RequestBattleSimulator();
        backBtn.OnClick();
    }

    private void Init()
    {
        view.equipmentSlotsDictionary = new Dictionary<EquipmentType, UIItemSlotBase>();
        foreach (var slot in view.uIEquipmentSlots)
        {
            view.equipmentSlotsDictionary.Add(slot.equipmentType, slot);
        }
    }

    protected override void LoadComponent()
    {
        base.LoadComponent();
        view = GetComponent<EnemyInfoPageView>();
    }

    public override void SetupDataInfo(List<ItemData> itemDatas)
    {
        if (setup == false)
        {
            Init();
            setup = true;
        }
        view.ShowAllChampion(itemDatas);
        bool showfirstItem = true;
        foreach (var item in view.uichamItems)
        {
            if (showfirstItem)
            {
                OnItemChampionClicked(item);
                showfirstItem = false;
            }
            item.OnItemClicked += OnItemChampionClicked;
        }
    }

    private void OnItemChampionClicked(UIItemSlotBase uiItem)
    {
        view.ShowData(uiItem.inventoryItem.data as HeroData);
    }
}