using System;
using Unity.Netcode;
using UnityEngine;


public class PlayerController : TGTHNetworkBehaviour
{
    private IStateMachine m_playerSM;
    private ActorController m_actorController;
    [HideInInspector] public IMoveable moveable;
    [HideInInspector] public Animator anim;
    override protected void Awake()
    {
        base.Awake();
        LoadComponent();
        m_playerSM = new PlayerStateMachine(this);
        m_playerSM.Init<IdleState_Player>();

    }
    override protected void Start()
    {
        base.Start();
        moveable = m_actorController.moveable;
    }
    private void Update()
    {
        if (!IsOwner) return;
        m_playerSM.Update();
    }
    override protected void LoadComponent()
    {
        base.LoadComponent();
        anim = GetComponentInChildren<Animator>();
        m_actorController = GetComponent<ActorController>();
    }
}