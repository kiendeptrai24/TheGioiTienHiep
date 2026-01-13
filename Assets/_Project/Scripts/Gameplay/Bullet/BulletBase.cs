using System.Collections;
using Unity.Netcode;
using UnityEngine;

public abstract class BulletBase : TGTHNetworkBehaviour
{
    protected ISkillCaster caster;
    protected StatsData statsData;
    protected Transform targetToChase;
    public void SetUpTarGet(ISkillCaster caster, Transform target, StatsData statsData)
    {
        this.caster = caster;
        this.targetToChase = target;
        this.statsData = statsData;
    }
    protected virtual void OnHit(Collider col)
    {
        if (!IsServer) return;
        if (col.TryGetComponent(out IDamageable target))
        {
            target.TakeDamage(statsData);
        }
    }
    protected bool IsTeam(ISkillCaster caster)
    {
        if (caster.TeamId == this.caster.TeamId)
        {
            return true;
        }
        return false;
    }
    protected virtual void OnBulletDespawn(float delay = 0f)
    {
        if (!IsServer) return;

        if (delay <= 0f)
        {
            NetworkObject.Despawn();
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(DespawnAfterDelay(delay));
        }
    }

    private IEnumerator DespawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        NetworkObject.Despawn();
    }
}