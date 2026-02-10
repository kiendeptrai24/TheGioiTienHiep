
public class Animationtrigger_Champion : TGTHMonoBehaviour
{
    protected ChampionController m_champion;
    protected IStateMachine m_heroSM;
    protected override void Awake()
    {
        m_champion = GetComponentInParent<ChampionController>();
    }
    override protected void Start()
    {
        base.Start();
        m_heroSM = m_champion.GetStateMachine();
    }
    public virtual void Animtiontrigger()
    {
        m_heroSM.GetFeature<IAnimationTrigger>()?.ActiveTrigger();
    }
    public virtual void ActiveSkill()
    {
        m_heroSM.GetFeature<ISkillTrigger>()?.ActiveSkill();
    }
}