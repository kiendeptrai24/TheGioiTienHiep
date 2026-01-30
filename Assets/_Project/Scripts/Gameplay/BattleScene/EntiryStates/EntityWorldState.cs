using Unity.VisualScripting;
using UnityEngine;

public abstract class EntityWorldState : IState
{
    protected EntityWorldController m_entity;
    protected IStateMachine m_machine;
    private Animator m_anim;
    protected string m_animName;
    protected float m_stateTimer;


    public EntityWorldState(EntityWorldController entity, IStateMachine stateMachine, string animName)
    {
        m_entity = entity;
        m_machine = stateMachine;
        m_animName = animName;
        m_anim = entity.anim;
    }
    
    public virtual void Enter()
    {
        m_stateTimer = 0;
        m_anim.SetBool(m_animName, true);
    }

    public virtual void Excute()
    {
        m_stateTimer -= Time.deltaTime;
    }

    public virtual void Exit()
    {
        m_anim.SetBool(m_animName, false);
    }
}