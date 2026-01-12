using UnityEngine;

public class AttackState_Hero : HeroState, ISkillTrigger, IAnimationTrigger
{
    private SkillContext skillContext;
    public AttackState_Hero(HeroController hero, IStateMachine stateMachine, string anim) : base(hero, stateMachine, anim)
    {

    }

    public void ActiveSkill()
    {

        var spawnPoint = new SpawnPoint();
        spawnPoint.position = m_hero.transform.position + Vector3.up * 1f;
        spawnPoint.rotation = m_hero.transform.rotation;
        skillContext = new SkillContext(m_hero.skillController.timeProvider, m_hero, m_hero, new SkillRuntime(), spawnPoint);
        Collider[] colliders = Physics.OverlapSphere(
                m_hero.transform.position,
            3
        );
        Debug.Log(m_hero.GetStats().AttackRange);
        var slash = GameObject.Instantiate(m_hero.attackPrefab, spawnPoint.position, spawnPoint.rotation);
        GameObject.Destroy(slash.gameObject, 1);
        foreach (Collider col in colliders)
        {
            if (col.TryGetComponent<ISkillCaster>(out var caster))
            {
                if (caster.TeamId == m_hero.TeamId)
                {
                    continue;
                }
            }
            if (col.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(skillContext, m_hero.GetStats());
            }
        }
    }

    public void ActiveTrigger()
    {
        m_machine.ChangeState<IdleState_Hero>();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Excute()
    {
        base.Excute();
    }

    public override void Exit()
    {
        base.Exit();
    }
}