
using UnityEngine;

public class DamageAreaBase : BulletBase
{
    protected override void Start()
    {
        base.Start();
        var hits = Physics.OverlapSphere(transform.position, 3);
        foreach (var col in hits)
        {
            var caster = col.GetComponent<ISkillCaster>();
            if (caster == null || IsTeam(caster)) continue;
            OnHit(col);
        }
        OnBulletDespawn(1);
    }
    private void OnTriggerEnter(Collider other)
    {
        OnHit(other);
    }
}