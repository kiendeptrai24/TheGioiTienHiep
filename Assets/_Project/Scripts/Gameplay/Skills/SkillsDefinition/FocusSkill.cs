
using UnityEngine;

public class FocusSkill : BaseSkill
{
    public FocusSkill(SkillData data, GameObject skillEffectPrefab, string skillId, string displayName, float cooldownSeconds = 1f)
        : base(data, skillEffectPrefab, skillId, displayName, cooldownSeconds)
    {
    }

    protected override void OnApplyEffect(in SkillContext ctx)
    {
        Vector3 targetPosition = ctx.Target.Center;
        Quaternion targetRotation = ctx.Target.Rotation;
        var fireball = GameObject.Instantiate(skillEffectPrefab, targetPosition, targetRotation);
        var projectile = fireball.GetComponent<ParticleSystem>();
        if (projectile != null)
            projectile.Play();
            Collider[] colliders = Physics.OverlapSphere(
                targetPosition,
                5
            );
            
            foreach (Collider col in colliders)
            {
                if(col.TryGetComponent<ISkillCaster>(out var caster))
                {
                    if(caster.TeamId == ctx.Caster.TeamId)
                    {
                        continue;
                    }
                }
                if (col.TryGetComponent<IDamageable>(out var damageable))
                {
                    damageable.TakeDamage(ctx, new StatsData());
                }
            }
        GameObject.Destroy(fireball, 1f);
    }
    override public void BuildDefaultConditions()
    {
        base.BuildDefaultConditions();
    }
}