using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRespawnTimerManager : SingletonNetwork<SpawnRespawnTimerManager>
{
    private readonly SortedSet<RespawnTask> respawnTasks = new();
    public List<RespawnTask> respawnTasksList = new();
    private long nextId;

    public void AddRespawnTask(long delay, Action onRespawn)
    {
        if (!IsServer) return;
        if (onRespawn == null) return;
        var task = new RespawnTask
        {
            Id = nextId++,
            SpawnTime = TimeUtils.DateTimeOffset(delay),
            OnRespawn = onRespawn
        };
        respawnTasks.Add(task);
        respawnTasksList.Add(task);
    }

    private void Update()
    {
        if (!IsServer) return;

        CheckRespawnTasks();
    }

    private void CheckRespawnTasks()
    {
        if (respawnTasks.Count == 0)
            return;

        double now = TimeUtils.DateTimeOffset();

        while (respawnTasks.Count > 0)
        {
            RespawnTask task = respawnTasks.Min;

            if (now < task.SpawnTime)
                break;

            respawnTasks.Remove(task);
            task.OnRespawn?.Invoke();
        }
    }

    public void Clear()
    {
        if (!IsServer) return;

        respawnTasks.Clear();
    }
}