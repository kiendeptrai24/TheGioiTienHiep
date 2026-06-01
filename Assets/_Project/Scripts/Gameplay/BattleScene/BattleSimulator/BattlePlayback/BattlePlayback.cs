using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon.StructWrapping;
using Unity.Mathematics;
using Unity.Profiling;
using Unity.VisualScripting;
using UnityEngine;

public class BattlePlayback : Singleton<BattlePlayback>
{
    [Serializable]
    public class ChampionSetup
    {
        public string championId;
        public ChampionController champion;
    }
    [Serializable]
    public class SkillSetup
    {
        public string skillId;
        public GameObject skillPrefab;
    }
    public float timeToDeplay = 0.5f;
    public int[,] framesUnit = new int[10, 10];
    public Transform origin;
    public List<ChampionSetup> championsSetup;
    public List<SkillSetup> skillsSetup;
    public List<BattleEvent> curEvents;
    public Vector2 offsetOrigin = new Vector2(1, 1);
    public Vector2 posOrigin = new Vector2(-5, -5);
    public bool playBattle = false;
    private float battleTimer = 0f;
    private int currentEventIndex = 0;
    private Dictionary<string, ChampionAnimationPlayback> champions = new();
    private Dictionary<string, ChampionAnimationPlayback> championsEnemies = new();
    private Dictionary<string, ChampionController> championsObject = new();
    private Dictionary<string, SkillSetup> skillsObject = new();
    public event Action OnEndBattle;
    public event Action OnResultGame;
    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
        foreach (var setup in championsSetup)
        {
            championsObject.Add(setup.championId, setup.champion);
        }
        foreach (var setup in skillsSetup)
        {
            skillsObject.Add(setup.skillId, setup);
        }
    }

    public void SetBattleEvents(List<BattleEvent> events)
    {
        curEvents = events;
        BattlePlaybackManager.Instance.ReadyGame();
        InitChampions();
    }
    public void StartBattle()
    {
        playBattle = true;
        battleTimer = Time.time;
    }

    private void InitChampions()
    {
        if (curEvents == null || curEvents.Count == 0)
        {
            Debug.Log("No battle events to play.");
            return;
        }
        List<BattleEventInit> eventsInit = new();

        foreach (var eventInit in curEvents)
        {
            if (eventInit is BattleEventInit)
            {
                eventsInit.Add(eventInit as BattleEventInit);
            }
        }
        foreach (var eventInit in eventsInit)
        {
            if (!championsObject.TryGetValue(eventInit.ownerUid, out var champion))
                continue;

            Vector3 rotOffset = eventInit.team == TeamId.Heroes ? Vector3.forward : Vector3.back;
            Quaternion rot = Quaternion.LookRotation(rotOffset);

            Vector3 boardOrigin = new Vector3(
                origin.position.x + posOrigin.x * offsetOrigin.x,
                0f,
                origin.position.z + posOrigin.y * offsetOrigin.y
            );

            Vector3 pos = BattleBoardLayout.CellToWorld(
                eventInit.cell,
                offsetOrigin.x,
                offsetOrigin.y,
                boardOrigin
            );

            var cham = Instantiate(champion, pos, rot);
            var chamAnim = cham.GetComponent<ChampionAnimationPlayback>();
            var statData = cham.GetComponent<StatsData>();
            var aiMovement = cham.GetComponent<AIChampionMovement>();
            aiMovement.Setspeed(eventInit.moveSpeed);
            var championData = GameDataCenterManager.Instance.GetItemById(eventInit.ownerUid);
            HeroData heroData = championData as HeroData;
            if (heroData != null && heroData.isCharacter)
            {
                heroData.skillDatas.Clear();
                foreach (var skillId in eventInit.skillIds)
                {
                    var skillData = GameDataCenterManager.Instance.GetItemById(skillId);
                    if (skillData is SkillData skill)
                    {
                        heroData.skillDatas.Add(skill);
                    }
                }
            }

            foreach (var skillData in heroData.skillDatas)
            {
                skillData.skillEffectPrefab = skillsObject[skillData.instanceId].skillPrefab;
            }
            statData.SetUpItem(championData);
            chamAnim.GetComponent<ChampionController>().SetTeamId((int)eventInit.team);
            if (eventInit.team == TeamId.Heroes)
                champions.Add(eventInit.ownerUid, chamAnim);
            else
                championsEnemies.Add(eventInit.ownerUid, chamAnim);
        }
    }
    public IEnumerator CheckChampion()
    {
        yield return new WaitForSeconds(.1f);

        bool heroesAlive = champions.Values.Any(c => c != null && IsChampionAlive(c));
        bool enemiesAlive = championsEnemies.Values.Any(c => c != null && IsChampionAlive(c));

        if (!heroesAlive || !enemiesAlive)
        {
            OnResultGame?.Invoke();
        }
    }

    private IEnumerator MonitorUntilCompletion()
    {
        float timeElapsed = 0f;
        float maxWaitTime = 5f;

        while (timeElapsed < maxWaitTime)
        {
            yield return new WaitForSeconds(0.1f);
            timeElapsed += 0.1f;

            bool heroesAlive = champions.Values.Any(c => c != null && IsChampionAlive(c));
            bool enemiesAlive = championsEnemies.Values.Any(c => c != null && IsChampionAlive(c));

            if (!heroesAlive || !enemiesAlive)
            {
                OnResultGame?.Invoke();
                yield break;
            }
        }

        OnResultGame?.Invoke();
    }

    private bool IsChampionAlive(ChampionAnimationPlayback champion)
    {
        if (champion == null || champion.gameObject == null) return false;
        var health = champion.GetComponent<HealthController>();
        return health != null;
    }
    public void StopBattle()
    {
        playBattle = false;
        StartCoroutine(OnEndGame(0));
    }
    private void ResetBattle()
    {
        playBattle = false;
        currentEventIndex = 0;

        var enemies = championsEnemies.Where(c => c.Value != null);
        var heroes = champions.Where(c => c.Value != null);
        foreach (var hero in heroes)
        {
            Destroy(hero.Value.gameObject);
        }
        foreach (var enemy in enemies)
        {
            Destroy(enemy.Value.gameObject);
        }
        champions.Clear();
        championsEnemies.Clear();
    }
    public IEnumerator OnEndGame(float timeToDeplay = 0)
    {
        yield return new WaitForSeconds(timeToDeplay / 2);
        ResetBattle();
        yield return new WaitForSeconds(timeToDeplay);
        OnEndBattle?.Invoke();
        CameraSwitchManager.Instance.ResetToPlayer();
    }


    void Update()
    {
        if (!playBattle) return;
        if (currentEventIndex < curEvents.Count &&
           Time.time - battleTimer >= curEvents[currentEventIndex].time)
        {
            Dispatch(curEvents[currentEventIndex]);
            currentEventIndex++;
        }
    }
    void Dispatch(BattleEvent e)
    {
        switch (e.type)
        {
            case BattleEventType.Move:
                PlayMovement(e);
                break;
            case BattleEventType.Attack:
                PlayAttack(e);
                break;
            case BattleEventType.Skill:
                PlayAnimationSkill(e);
                break;
            case BattleEventType.Death:
                PlayDeath(e);
                break;
            case BattleEventType.End:
                playBattle = false;
                StartCoroutine(MonitorUntilCompletion());
                break;
            default:
                break;
        }
    }
    private IEnumerator DecreaseHealthBattle(BattleEvent e)
    {
        var bea = e as BattleEventAttack;
        if (bea == null)
            yield break;

        var atkCham = GetAnimationCham(bea.attackerUid, bea.team);
        var defCham = GetAnimationCham(bea.targetUid, bea.targetTeam);
        float delay = GetHealthDecreaseDelay(bea, atkCham, defCham);
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        var chamAnim = GetAnimationCham(bea.targetUid, bea.targetTeam);
        if (chamAnim == null)
            yield break;

        var health = chamAnim.GetComponent<HealthController>();
        if (health == null)
            yield break;

        int damage = bea.damage;
        health.DecreaseHealth(damage, 0);
        DamagePopupSpawner.Instance.Spawn(damage, chamAnim.transform, bea.isCrit);
    }

    private float GetHealthDecreaseDelay(BattleEventAttack bea, ChampionAnimationPlayback atkCham, ChampionAnimationPlayback defCham)
    {
        float delay = Mathf.Max(0f, bea.castTime);
        if (atkCham == null || defCham == null)
            return delay;

        var attackerController = atkCham.GetComponent<ChampionController>();
        if (attackerController == null || attackerController.isMeleeChampion)
            return delay;

        float travel = GetProjectileTravelTime(attackerController, defCham);
        return delay + travel;
    }

    private float GetProjectileTravelTime(ChampionController attacker, ChampionAnimationPlayback defCham)
    {
        if (attacker.attackPrefab == null || defCham == null)
            return 0f;

        float speed = GetAttackPrefabMoveSpeed(attacker.attackPrefab);
        if (speed <= 0f)
            return 0f;

        float distance = Vector3.Distance(attacker.transform.position, defCham.transform.position);
        return distance / speed;
    }

    private float GetAttackPrefabMoveSpeed(GameObject attackPrefab)
    {
        if (attackPrefab.TryGetComponent<OneHitBulletPlayback>(out var playbackBullet))
            return playbackBullet.MoveSpeed;
        if (attackPrefab.TryGetComponent<BulletPlayBackBase>(out var playbackBase))
            return playbackBase.MoveSpeed;
        return 0f;
    }
    public void PlayAnimationSkill(BattleEvent e)
    {
        var skill = e as BattleEventSkill;
        if (skill == null)
            return;
        var atkCham = GetAnimationCham(skill.attackerUid, skill.team);
        var defCham = GetAnimationCham(skill.targetUid, skill.targetTeam);
        if (atkCham == null || defCham == null)
            return;
        atkCham.GetComponent<TargetFinderBase>().SetTarget(defCham.transform);
        atkCham.PlayAnimationSkill(skill.skillId);
        StartCoroutine(DecreaseHealthBattle(skill));
    }
    public void PlayMovement(BattleEvent e)
    {
        var eventMove = e as BattleEventMove;
        if (eventMove == null) return;

        var ownerCham = GetAnimationCham(eventMove.ownerUid, eventMove.team);
        var targetCham = GetAnimationCham(eventMove.targetUid, eventMove.targetTeam);
        if (ownerCham == null || targetCham == null)
            return;

        Vector3 boardOrigin = new Vector3(
            origin.position.x + posOrigin.x * offsetOrigin.x,
            0f,
            origin.position.z + posOrigin.y * offsetOrigin.y
        );

        Vector3 destination = BattleBoardLayout.CellToWorld(
            eventMove.to,
            offsetOrigin.x,
            offsetOrigin.y,
            boardOrigin
        );

        lastDestination = destination;
        hasDestination = true;

        ownerCham.GetComponent<TargetFinderBase>().SetTarget(targetCham.transform);
        ownerCham.PlayMovement(destination);
    }
    public void PlayAttack(BattleEvent e)
    {
        var eventAttack = e as BattleEventAttack;
        if (eventAttack == null)
            return;
        var atkCham = GetAnimationCham(eventAttack.attackerUid, eventAttack.team);
        var defCham = GetAnimationCham(eventAttack.targetUid, eventAttack.targetTeam);
        if (atkCham == null || defCham == null)
            return;
        atkCham.GetComponent<TargetFinderBase>().SetTarget(defCham.transform);
        atkCham.PlayAnimationAttack();
        StartCoroutine(DecreaseHealthBattle(e));
    }
    public void PlayDeath(BattleEvent e)
    {

    }
    private ChampionAnimationPlayback GetAnimationCham(string chamId, TeamId team)
    {
        if (team == TeamId.Heroes && champions.TryGetValue(chamId, out var champ))
            return champ;
        if (team == TeamId.Enemies && championsEnemies.TryGetValue(chamId, out var champEnemy))
            return champEnemy;

        return null;
    }
    public void SetBattleTimer(float time)
    {
        TimeScaleManager.SetUnityTimeScale(time);
    }
    override protected void LoadComponent()
    {
        base.LoadComponent();
    }
    private Vector3 lastDestination;
    private bool hasDestination;
    private void OnDrawGizmos()
    {
        if (!hasDestination) return;

        Gizmos.color = Color.green;
        var pos = lastDestination;
        pos.y = 1;
        Gizmos.DrawSphere(pos, 1f);
    }
}
