using UnityEngine;


public class OneHitBulletPlayback : BulletPlayBackBase
{
    [SerializeField] protected float moveSpeed;
    public override float MoveSpeed => moveSpeed;

    private void FixedUpdate()
    {
        if (targetToChase == null) return;
        transform.position = Vector3.MoveTowards(transform.position, targetToChase.transform.position, moveSpeed * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        var caster = other.GetComponent<ISkillCaster>();
        if (caster == null || IsTeam(caster)) return;
        OnHit(other);
        OnBulletDespawn(.5f);
    }
}