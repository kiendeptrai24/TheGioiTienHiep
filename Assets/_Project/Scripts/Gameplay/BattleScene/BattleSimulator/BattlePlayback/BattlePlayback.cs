using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.Profiling;
using UnityEngine;

public class BattlePlayback : Singleton<BattlePlayback>
{
    public float timeToDeplay = 0.5f;
    public int[,] framesUnit = new int[10, 10];
    public Transform origin;
    public List<ChampionController> objects;
    public List<BattleEvent> curEvents;
    public Vector2 offsetOrigin = new Vector2(1, 1);
    public Vector2 posOrigin = new Vector2(-5, -5);
    public bool playBattle = false;
    private float battleTimer = 0f;
    private int currentEventIndex = 0;
    private Dictionary<string, ChampionAnimationPlayback> champions = new();
    private Dictionary<string, ChampionAnimationPlayback> championsEnemies = new();
    private Dictionary<string, ChampionController> championsObject = new();
    public event Action OnEndBattle;
    public event Action OnResultGame;
    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
        foreach (var champ in objects)
        {
            var stats = champ.GetComponent<StatsData>();
            string id = stats.heroPreset.itemId;
            championsObject.Add(id, champ);
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

        OnResultGame?.Invoke();
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
                StartCoroutine(CheckChampion());
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
        yield return new WaitForSeconds(bea.castTime);

        var chamAnim = GetAnimationCham(bea.targetUid, bea.targetTeam);
        if (chamAnim == null)
            yield break;

        var health = chamAnim.GetComponent<HealthController>();
        if (health == null)
            yield break;

        int damage = bea.damage;
        health.DecreaseHealth(damage, 0);
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
        atkCham.PlayAnimationAttack();
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
