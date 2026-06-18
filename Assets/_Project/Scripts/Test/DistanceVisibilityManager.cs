using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DistanceVisibilityManager : SingletonNetwork<DistanceVisibilityManager>
{
    [Header("Default Distance")]
    public float defaultMaxDistance = 10f;
    public float checkInterval = 0.5f;

    private readonly List<NetworkVisibilityChecker> _checkers = new();
    [SerializeField] private List<NetworkVisibilityChecker> _checkersTest = new();
    private readonly HashSet<ulong> _pendingClientRefreshes = new();
    private readonly List<ulong> _resolvedClientIds = new();
    private float _timer;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsServer || NetworkManager.Singleton == null) return;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer && NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }

        _pendingClientRefreshes.Clear();
        base.OnNetworkDespawn();
    }

    public void Register(NetworkVisibilityChecker checker)
    {
        if (checker == null || _checkers.Contains(checker)) return;

        _checkers.Add(checker);
        _checkersTest.Add(checker);
        RefreshVisibilityForAllClients(checker);
    }

    public void Unregister(NetworkVisibilityChecker checker)
    {
        _checkers.Remove(checker);
        _checkersTest.Remove(checker);
    }

    private void Update()
    {
        if (!IsServer) return;
        if (Time.time < _timer + checkInterval) return;

        _timer = Time.time;
        ProcessPendingClientRefreshes();
        CheckAllVisibility();
    }

    public void RefreshVisibilityForAllClients(NetworkVisibilityChecker checker)
    {
        if (!IsServer || checker == null || !checker.IsSpawned) return;
        if (NetworkManager.Singleton == null || NetworkManager.Singleton.ConnectedClientsList == null) return;

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            UpdateVisibilityForClient(checker, client.ClientId, client.PlayerObject);
        }
    }

    public void RefreshVisibilityForClient(ulong clientId)
    {
        if (!IsServer || NetworkManager.Singleton == null) return;
        if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var client)) return;

        if (client.PlayerObject == null)
        {
            _pendingClientRefreshes.Add(clientId);
            return;
        }

        foreach (var checker in _checkers)
        {
            UpdateVisibilityForClient(checker, clientId, client.PlayerObject);
        }
    }

    private void CheckAllVisibility()
    {
        if (NetworkManager.Singleton == null || NetworkManager.Singleton.ConnectedClientsList == null) return;

        foreach (var checker in _checkers)
        {
            if (checker == null || !checker.IsSpawned) continue;

            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                UpdateVisibilityForClient(checker, client.ClientId, client.PlayerObject);
            }
        }
    }

    private void UpdateVisibilityForClient(NetworkVisibilityChecker checker, ulong clientId, NetworkObject clientPlayerObject)
    {
        if (checker == null || !checker.IsSpawned) return;
        if (clientPlayerObject == null) return;
        if (clientId == checker.OwnerClientId) return;

        var networkObject = checker.NetworkObject;
        if (networkObject == null || !networkObject.IsSpawned) return;

        float maxDistance = checker.distance > 0f ? checker.distance : defaultMaxDistance;
        float distance = Vector3.Distance(checker.transform.position, clientPlayerObject.transform.position);
        bool isVisible = networkObject.IsNetworkVisibleTo(clientId);

        if (distance > maxDistance)
        {
            if (isVisible)
            {
                networkObject.NetworkHide(clientId);
            }

            return;
        }

        if (!isVisible)
        {
            networkObject.NetworkShow(clientId);
        }
    }

    private void ProcessPendingClientRefreshes()
    {
        if (_pendingClientRefreshes.Count == 0 || NetworkManager.Singleton == null) return;

        _resolvedClientIds.Clear();

        foreach (var clientId in _pendingClientRefreshes)
        {
            if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var client)) continue;
            if (client.PlayerObject == null) continue;

            foreach (var checker in _checkers)
            {
                UpdateVisibilityForClient(checker, clientId, client.PlayerObject);
            }

            _resolvedClientIds.Add(clientId);
        }

        foreach (var clientId in _resolvedClientIds)
        {
            _pendingClientRefreshes.Remove(clientId);
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        _pendingClientRefreshes.Add(clientId);
    }

    private void OnClientDisconnected(ulong clientId)
    {
        _pendingClientRefreshes.Remove(clientId);
    }
}
