using System;
using Unity.Netcode;
using UnityEngine;


public class HeroRangeController : HeroController
{
    override protected void Awake()
    {
        LoadComponent();
        healthController.OnDead += OnDeadServerRpc;
        m_heroLoadData.OnHeroDataLoaded += LoadHeroData;
        m_heroSM = new HeroRangeStateMachine(this);
        m_heroSM.Init<IdleState_Hero>();
    }

    override protected void Start()
    {
        base.Start();
    }

    private void Update()
    {
        if (!IsOwner) return;
        m_heroSM.Update();
    }

    override protected void LoadComponent()
    {
        base.LoadComponent();
    }
}