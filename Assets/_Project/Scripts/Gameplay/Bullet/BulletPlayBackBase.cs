using System.Collections;
using Unity.Netcode;
using UnityEngine;

public abstract class BulletPlayBackBase : TGTHMonoBehaviour
{
    protected ISkillCaster caster;
    protected StatsData statsData;
    protected Transform targetToChase;
    public virtual float MoveSpeed => 0f;
    public void SetUpTarGet(ISkillCaster caster, Transform target, StatsData statsData)
    {
        this.caster = caster;
        this.targetToChase = target;
        this.statsData = statsData;
    }
    protected virtual void OnHit(Collider col)
    {

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
        if (delay <= 0f)
        {
            ObjectPool.Instance.ReturnObject(this.gameObject);
        }
        else
        {
            ObjectPool.Instance.ReturnObject(this.gameObject, delay);
        }
    }

}