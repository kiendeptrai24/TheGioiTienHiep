
using UnityEngine;

public class EntityWorldController : TGTHNetworkBehaviour
{
    public Animator anim;
    protected IStateMachine m_entitySM;
    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
    }
    protected override void Start()
    {
        base.Start();
    }
    override public void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer) return;
        Debug.Log($"Monster Spawned: {NetworkObjectId}");
    }
    private void Update()
    {
        if (!IsOwner || m_entitySM == null) return;
        m_entitySM.Update();
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
        anim = GetComponentInChildren<Animator>();
    }
}