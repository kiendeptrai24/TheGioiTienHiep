using Unity.Netcode;
using UnityEngine;

public class NetworkConnectionEvents : TGTHNetworkBehaviour
{
    private StatsData statsData;
    private ClientManager clientManager;
    private string characterId;
    private bool isConnected;


    // =========================
    // UNITY
    // =========================

    protected override void Awake()
    {
        base.Awake();

        LoadComponent();

        statsData.OnStatReady += OnStatReady;
        clientManager = ClientManager.Instance;
    }

    protected new void OnDestroy()
    {
        statsData.OnStatReady -= OnStatReady;
    }

    // =========================
    // NETWORK
    // =========================

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsOwner)
            return;

        TryNotifyPlayerConnected();
    }

    // =========================
    // EVENTS
    // =========================
    private void OnStatReady(StatsData data)
    {
        var heroData = data.chamionData as HeroData;

        if (heroData == null)
            return;

        characterId = heroData.characterId;

        TryNotifyPlayerConnected();
    }

    // =========================
    // CONNECT / DISCONNECT
    // =========================

    private void TryNotifyPlayerConnected()
    {
        if (IsSpawned == false) return;
        if (!IsOwner && isConnected)
            return;
        if (string.IsNullOrEmpty(characterId))
            return;
        isConnected = true;
        OnPlayerConnectedServerRpc(characterId, NetworkManager.LocalClientId);
    }

    // =========================
    // RPC
    // =========================

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    private void OnPlayerConnectedServerRpc(string characterId, ulong LocalClientId)
    {
        clientManager.OnClientConnected(characterId, LocalClientId);
    }

    // =========================
    // LOAD
    // =========================

    protected override void LoadComponent()
    {
        base.LoadComponent();
        statsData = GetComponent<StatsData>();
    }
}