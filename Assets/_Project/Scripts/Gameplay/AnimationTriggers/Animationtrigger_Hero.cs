using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class Animationtrigger_Hero : TGTHMonoBehaviour
{
    protected HeroController m_hero;
    protected IStateMachine m_heroSM;
    protected override void Awake()
    {
        m_hero = GetComponentInParent<HeroController>();
    }
    override protected void Start()
    {
        base.Start();
        m_heroSM = m_hero.GetStateMachine();
    }
    public virtual void Animtiontrigger()
    {
        if (!m_hero.IsServer) return;
        m_heroSM.GetFeature<IAnimationTrigger>()?.ActiveTrigger();
    }
    public virtual void ActiveSkill()
    {
        if (!m_hero.IsServer) return;
        m_heroSM.GetFeature<ISkillTrigger>()?.ActiveSkill();
    }
}