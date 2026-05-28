using System.Collections.Generic;
using UnityEngine;

public class ScreenManagerHub : Singleton<ScreenManagerHub>
{
    private readonly Dictionary<string, ScreenManager> managers = new();

    public void Register(string key, ScreenManager manager)
    {
        if (manager == null) return;

        managers[key] = manager;
    }

    public void Unregister(string key)
    {
        if (managers.ContainsKey(key))
            managers.Remove(key);
    }

    public ScreenManager Get(string key)
    {
        managers.TryGetValue(key, out var manager);
        return manager;
    }

    public void ResetAll()
    {
        foreach (var manager in managers.Values)
        {
            manager.ResetNavigation();
        }
    }

    public void HideAll()
    {
        foreach (var manager in managers.Values)
        {
            manager.HideAll();
        }
    }
}