using Unity.Netcode;
using UnityEngine;

public class PlayerBattleState : NetworkBehaviour
{
    public NetworkVariable<bool> InBattle = new(false);
    public NetworkVariable<int> SessionId = new(0);

    // Optional: lưu monster đang engage để tránh spam
    public NetworkVariable<ulong> EngagedMonsterNetId = new(0);

    public void ServerEnterBattle(int sessionId, ulong monsterNetId)
    {
        if (!IsServer) return;
        InBattle.Value = true;
        SessionId.Value = sessionId;
        EngagedMonsterNetId.Value = monsterNetId;
    }

    public void ServerExitBattle()
    {
        if (!IsServer) return;
        InBattle.Value = false;
        SessionId.Value = 0;
        EngagedMonsterNetId.Value = 0;
    }
}
