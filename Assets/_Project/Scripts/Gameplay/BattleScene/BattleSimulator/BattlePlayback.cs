using System.Collections.Generic;
using UnityEngine;

public class BattlePlayback : TGTHMonoBehaviour
{
    public int[,] framesUnit = new int[10, 10];
    public Transform origin;
    public List<ChampionController> objects;
    public List<BattleEvent> events;
    public Vector2 offsetOrigin = new Vector2(1, 1);
    public Vector2 posOrigin = new Vector2(-5, -5);
    public bool playBattle = false;
    private float battleTimer = 0f;
    private int currentEventIndex = 0;
    private Dictionary<string, ChampionAnimationPlayback> champions = new();
    private Dictionary<string, ChampionAnimationPlayback> championsEnemies = new();
    private Dictionary<string, ChampionController> championsObject = new();
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
    [ContextMenu("set battle events")]
    public void SetBattleEvent()
    {
        events = BattleSimulatorRequest.Instance.battleEvents;
        StartBattle();
    }
    [ContextMenu("start battle events")]
    public void StartBattle()
    {
        playBattle = true;
        List<BattleEventInit> eventsInit = new();

        foreach (var eventInit in events)
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

            Vector3 rotOffset = eventInit.team == TeamId.Heroes ? Vector3.zero : Vector3.back;
            Quaternion rot = Quaternion.LookRotation(rotOffset);

            float xPos = Mathf.RoundToInt(origin.position.x + eventInit.cell.y);
            float yPos = Mathf.RoundToInt(origin.position.z + eventInit.cell.x);

            Vector3 pos = new Vector3(
                (xPos + posOrigin.x) * offsetOrigin.x,
                0,
                (yPos + posOrigin.y) * offsetOrigin.y
            );
            var cham = Instantiate(champion, pos, rot);
            var chamAnim = cham.GetComponent<ChampionAnimationPlayback>();
            if (eventInit.team == TeamId.Heroes)
                champions.Add(eventInit.ownerUid, chamAnim);
            else
                championsEnemies.Add(eventInit.ownerUid, chamAnim);
        }

        SetStartBattle();
    }
    private void ResetBattle()
    {
        playBattle = false;
        currentEventIndex = 0;
    }
    public void SetStartBattle()
    {
        playBattle = true;
        battleTimer = Time.time;
    }
    void Update()
    {
        if (!playBattle) return;
        if (currentEventIndex < events.Count &&
           Time.time - battleTimer >= events[currentEventIndex].time)
        {
            Debug.Log($"Dispatching event at time {events[currentEventIndex].type} for champion id {events[currentEventIndex].ownerUid}");
            Dispatch(events[currentEventIndex]);
            currentEventIndex++;
        }
        if (currentEventIndex >= events.Count)
        {
            ResetBattle();
        }
    }
    void Dispatch(BattleEvent e)
    {
        switch (e.type)
        {
            case BattleEventType.Move:
                PlayMovement(e, e.team);
                break;
            case BattleEventType.Attack:
                var attack = e as BattleEventAttack;
                DescreaseHealth(e, e.team);
                PlayAttack(GetAnimationCham(e.ownerUid, e.team));
                break;
            case BattleEventType.Skill:
                var skill = e as BattleEventSkill;
                PlayAnimationSkill(GetAnimationCham(e.ownerUid, e.team), skill.skillId);
                break;
            case BattleEventType.Death:
                PlayDeath(e.ownerUid, e.team);
                break;
        }
    }
    public void DescreaseHealth(BattleEvent chamId, TeamId team)
    {

        var battleEventAttack = chamId as BattleEventAttack;
        var chamAnim = GetAnimationCham(battleEventAttack.attackerUid, team);
        var health = chamAnim.GetComponent<HealthController>();
        int damage = battleEventAttack.damage;
        if (health == null)
        {
            Debug.Log($"Cannot find health component for champion id {battleEventAttack.attackerUid} in team {team}");
            return;
        }
        health.DecreaseHealth(damage, 0);
    }
    public void PlayAnimationSkill(ChampionAnimationPlayback chamAnim, string skillId)
    {
        chamAnim.PlayAnimationSkill(skillId);
    }
    public void PlayMovement(BattleEvent chamId, TeamId team)
    {
        var eventMove = chamId as BattleEventMove;
        var chamAnim = GetAnimationCham(eventMove.ownerUid, team);
        Vector2 cell = eventMove.to;
        Vector3 destination = new Vector3(
                (cell.x + posOrigin.x) * offsetOrigin.x,
                0,
                (cell.y + posOrigin.y) * offsetOrigin.y
            );
        chamAnim.PlayMovement(destination);
    }
    public void PlayAttack(ChampionAnimationPlayback chamAnim)
    {
        chamAnim.PlayAnimationAttack();
    }
    public void PlayDeath(string chamId, TeamId team)
    {
        var chamAnim = GetAnimationCham(chamId, team);
        Debug.Log($"Play death animation for champion id {chamId} in team {team}");
        chamAnim.PlayAnimationDeath();
        if (team == TeamId.Heroes)
        {
            champions.Remove(chamId);
        }
        else
        {
            championsEnemies.Remove(chamId);
        }
    }
    private ChampionAnimationPlayback GetAnimationCham(string chamId, TeamId team)
    {
        if (team == TeamId.Heroes && champions.TryGetValue(chamId, out var champ))
            return champ;
        if (team == TeamId.Enemies && championsEnemies.TryGetValue(chamId, out var champEnemy))
            return champEnemy;

        Debug.Log($"Cannot find champion animation with id {chamId} in team {team}");
        return null;
    }
}
