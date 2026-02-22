using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerBattleRoster : NetworkBehaviour
{
    [Header("Hero prefabs of this player (must be NetworkObject prefabs + registered in NetworkManager)")]
    public List<NetworkObject> heroNetPrefabs = new();
    public List<GameObject> chamPrefabs = new();
    // Bạn có thể thêm logic chọn đội hình (chỉ spawn N con đầu tiên)
    public int maxHeroesToSpawn = 5;
}
