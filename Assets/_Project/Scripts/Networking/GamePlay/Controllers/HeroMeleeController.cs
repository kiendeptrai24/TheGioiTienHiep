using System;
using Unity.Netcode;
using UnityEngine;


public class HeroMeleeController : HeroController
{
    override protected void Awake()
    {
        base.Awake();
        m_heroSM = new HeroMeleeStateMachine(this);
        m_heroSM.Init<IdleState_Hero>();
    }
}