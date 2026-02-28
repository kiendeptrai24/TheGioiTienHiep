using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MonsterBattleLoadout : NetworkBehaviour
{
    [Header("Enemies to spawn in battle for this world monster")]
    public List<NetworkObject> enemyPrefabs = new();
}
