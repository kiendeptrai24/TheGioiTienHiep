using UnityEngine;


public class OneHitBulletPlayback : BulletPlayBackBase
{
    [SerializeField] protected float moveSpeed;
    public override float MoveSpeed => moveSpeed;
    private float timeDestroy = 2f;
    private bool autoDestroy = true;
    private void OnEnable()
    {
        autoDestroy = true;
        Invoke(nameof(AutoDestroy), timeDestroy);
    }
    public void AutoDestroy()
    {
        if (autoDestroy)
            OnBulletDespawn();
    }
    private void FixedUpdate()
    {
        if (targetToChase == null) return;
        transform.position = Vector3.MoveTowards(transform.position, targetToChase.transform.position, moveSpeed * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        var caster = other.GetComponent<ISkillCaster>();
        if (caster == null || IsTeam(caster)) return;
        autoDestroy = false;
        OnHit(other);
        OnBulletDespawn(.5f);
    }
}