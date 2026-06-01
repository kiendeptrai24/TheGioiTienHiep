
public class MonsterWorldController : EntityWorldController
{
    protected override void Awake()
    {
        base.Awake();
        m_entitySM = new EnemyWorldStateMachine(this);
        m_entitySM.Init<IdleState_EntityWorld>();
    }
    protected override void Start()
    {
        base.Start();
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer) return;

        SpawnMonster.Instance.AddNetObject(NetworkObject);
    }
}