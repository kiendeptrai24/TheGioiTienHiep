using Unity.VisualScripting;
using UnityEngine;

public abstract class HeroState : IState
{
    protected HeroController m_hero;
    protected IStateMachine m_machine;
    private Animator m_anim;
    protected string m_animName;
    protected float m_stateTimer;


    public HeroState(HeroController hero, IStateMachine stateMachine, string animName)
    {
        m_hero = hero;
        m_machine = stateMachine;
        m_animName = animName;
        m_anim = hero.anim;
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