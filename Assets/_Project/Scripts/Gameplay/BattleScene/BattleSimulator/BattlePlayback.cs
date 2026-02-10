using System;
using System.Collections.Generic;
using UnityEngine;

public class BattlePlayback : MonoBehaviour
{
    public int[,] framesUnit = new int[10, 10];
    public Transform origin;
    public List<GameObject> objects;
    public List<BattleEvent> events;
    public bool playBattle = false;
    private float battleTimer = 0f;
    private Dictionary<string, GameObject> champions = new();

    void Start()
    {
        foreach (var cham in objects)
        {
            var stats = cham.GetComponent<StatsData>();
            string id = stats.heroPreset.itemId;
            Debug.Log(id);
            champions.Add(id, cham);
        }
        Debug.Log(champions.Count);
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
            if (!champions.TryGetValue(eventInit.ownerUid, out GameObject champion))
                continue;

            Vector2 posOffset = new Vector2(-5, -5);
            Vector3 rotOffset = eventInit.team == TeamId.Heroes ? Vector3.zero : Vector3.back;
            Quaternion rot = Quaternion.LookRotation(rotOffset);

            float xPos = Mathf.RoundToInt(origin.position.x + eventInit.cell.y);
            float yPos = Mathf.RoundToInt(origin.position.z + eventInit.cell.x);

            Vector3 pos = new Vector3(
                xPos + posOffset.x,
                0,
                yPos + posOffset.y
            );

            Instantiate(champion, pos, rot);
        }
        battleTimer = Time.time;
    }
    void Update()
    {
        if (!playBattle) return;

    }
    void Dispatch(BattleEvent e)
    {
        switch (e.type)
        {
        }
    }

}
