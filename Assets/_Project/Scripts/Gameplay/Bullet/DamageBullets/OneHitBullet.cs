using UnityEngine;


public class OneHitBullet : BulletBase
{
    [SerializeField] protected float moveSpeed;
    private void FixedUpdate()
    {
        if (!IsServer) return;
        if (targetToChase == null) return;
        transform.position = Vector3.MoveTowards(transform.position, targetToChase.transform.position, moveSpeed * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;
        var caster = other.GetComponent<ISkillCaster>();
        if (caster == null || IsTeam(caster)) return;
        OnHit(other);
        OnBulletDespawn(.5f);
    }
}