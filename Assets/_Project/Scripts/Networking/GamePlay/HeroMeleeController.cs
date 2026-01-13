using System;
using Unity.Netcode;
using UnityEngine;


public class HeroMeleeController : HeroController
{
    override protected void Awake()
    {
        base.Awake();
        LoadComponent();
        m_heroSM = new HeroMeleeStateMachine(this);
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

}