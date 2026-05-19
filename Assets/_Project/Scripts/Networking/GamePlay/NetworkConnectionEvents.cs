using Unity.Netcode;

public class NetworkConnectionEvents : TGTHNetworkBehaviour
{
    private StatsData statsData;
    private SegmentResourceManager segmentMineManager;

    private string characterId;

    private bool hasConnected;

    // =========================
    // UNITY
    // =========================

    protected override void Awake()
    {
        base.Awake();

        LoadComponent();

        statsData.OnStatReady += OnStatReady;
    }

    protected new void OnDestroy()
    {
        statsData.OnStatReady -= OnStatReady;

        if (IsOwner)
        {
            UnregisterNetworkEvents();
        }
    }

    // =========================
    // NETWORK
    // =========================

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsOwner)
            return;

        RegisterNetworkEvents();

        TryNotifyPlayerConnected();
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        if (!IsOwner)
            return;

        NotifyPlayerDisconnected();
    }

    // =========================
    // EVENTS
    // =========================

    private void RegisterNetworkEvents()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void UnregisterNetworkEvents()
    {
        if (NetworkManager.Singleton == null)
            return;

        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
    }

    private void OnStatReady(StatsData data)
    {
        var heroData = data.chamionData as HeroData;

        if (heroData == null)
            return;

        characterId = heroData.characterId;

        TryNotifyPlayerConnected();
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if (clientId != OwnerClientId)
            return;

        NotifyPlayerDisconnected();
    }

    // =========================
    // CONNECT / DISCONNECT
    // =========================

    private void TryNotifyPlayerConnected()
    {
        if (!IsOwner)
            return;
        if (string.IsNullOrEmpty(characterId))
            return;

        OnPlayerConnectedServerRpc(characterId);
    }

    private void NotifyPlayerDisconnected()
    {
        if (string.IsNullOrEmpty(characterId))
            return;

        OnPlayerDisconnectedServerRpc(characterId);
    }

    // =========================
    // RPC
    // =========================

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    private void OnPlayerConnectedServerRpc(string characterId)
    {
        segmentMineManager.OnPlayerReconnect(characterId, NetworkObjectId);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    private void OnPlayerDisconnectedServerRpc(string characterId)
    {
        segmentMineManager.OnPlayerDisconnect(characterId);
    }

    // =========================
    // LOAD
    // =========================

    protected override void LoadComponent()
    {
        base.LoadComponent();

        statsData = GetComponent<StatsData>();

        segmentMineManager = SegmentResourceManager.Instance;
    }
}