using UnityEngine;

public abstract class ChampionState : IState
{
    protected ChampionController m_champion;
    protected IStateMachine m_machine;
    private Animator m_anim;
    protected string m_animName;
    protected float m_stateTimer;


    public ChampionState(ChampionController champion, IStateMachine stateMachine, string animName)
    {
        m_champion = champion;
        m_machine = stateMachine;
        m_animName = animName;
        m_anim = champion.anim;
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