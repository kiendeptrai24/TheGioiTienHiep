using System;
using Unity.Netcode;
using UnityEngine;


public class HeroRangeController : HeroController
{
    override protected void Awake()
    {
        base.Awake();
        m_heroSM = new HeroRangeStateMachine(this);
        m_heroSM.Init<IdleState_Hero>();
    }
}