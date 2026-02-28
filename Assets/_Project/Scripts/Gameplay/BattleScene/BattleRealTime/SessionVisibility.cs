using Unity.Netcode;

public class SessionVisibility : NetworkBehaviour
{
    public int SessionId;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkObject.CheckObjectVisibility = CheckVisibility;
        }
    }

    private bool CheckVisibility(ulong clientId)
    {
        if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var cc)) return false;
        var po = cc.PlayerObject;
        if (po == null) return false;

        var st = po.GetComponent<PlayerBattleState>();
        if (st == null) return false;

        return st.InBattle.Value && st.SessionId.Value == SessionId;
    }
}
