
using System.Collections.Generic;
using UnityEngine;

public class SegmentUpgradeManager : Singleton<SegmentUpgradeManager>
{
    public List<UpgradeState> UpgradeStates = new();
    public Dictionary<string, ulong> clients = new();
    public void OnClientConect(string characterId, ulong ClientId)
    {
        if (!clients.ContainsKey(characterId))
        {
            clients.Add(characterId, ClientId);
        }
    }
    public void OnClientDisconect(string characterId, ulong ClientId)
    {
        if (clients.TryGetValue(characterId, out ulong clientId))
        {
            clients.Remove(characterId);
        }
    }
}