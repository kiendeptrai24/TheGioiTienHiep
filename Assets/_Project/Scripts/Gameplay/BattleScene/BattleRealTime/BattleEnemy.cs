using Unity.Netcode;
using UnityEngine;

public class BattleEnemy : NetworkBehaviour
{
    public int SessionId;

    // gọi hàm này khi enemy chết (server)
    public void ServerDie()
    {
        if (!IsServer) return;

        BattleService.Instance.ServerOnEnemyDead(SessionId);

        // despawn enemy
        if (NetworkObject != null && NetworkObject.IsSpawned)
            NetworkObject.Despawn(true);
    }
}
