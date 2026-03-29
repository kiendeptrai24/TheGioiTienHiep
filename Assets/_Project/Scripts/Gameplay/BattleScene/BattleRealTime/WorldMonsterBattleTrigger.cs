using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WorldMonsterBattleTrigger : NetworkBehaviour
{
    private void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        var playerNO = other.GetComponentInParent<NetworkObject>();
        if (playerNO == null || !playerNO.IsPlayerObject) return;

        var playerState = playerNO.GetComponent<PlayerBattleState>();
        if (playerState == null) return;

        // Nếu đang battle rồi thì bỏ qua
        if (playerState.InBattle.Value) return;

        var loadout = GetComponent<MonsterBattleLoadout>();
        if (loadout == null || loadout.enemyPrefabs.Count == 0) return;
        //BattleSimulatorRequest.Instance.RequestBattleSimulatorServerRpc(playerNO.NetworkObjectId, NetworkObjectId);
    }

}
