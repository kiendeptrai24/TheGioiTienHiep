using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class DistanceVisibilityManager : SingletonNetwork<DistanceVisibilityManager>
{

    [Header("Default Distance")]
    public float defaultMaxDistance = 5f;
    public float checkInterval = 0.5f;

    private readonly List<NetworkVisibilityChecker> _checkers = new();
    [SerializeField] private List<NetworkVisibilityChecker> _checkersTest = new();
    private float _timer;

    public void Register(NetworkVisibilityChecker checker)
    {
        _checkersTest.Add(checker);
        _checkers.Add(checker);
    }
    public void Unregister(NetworkVisibilityChecker checker)
    {
        _checkersTest.Remove(checker);
        _checkers.Remove(checker);
    }

    private void Update()
    {
        if (!IsServer) return;

        if (Time.time < _timer + checkInterval) return;
        _timer = Time.time;

        CheckAllVisibility();
    }

    private void CheckAllVisibility()
    {
        // Lấy tất cả connected clients
        if (NetworkManager.Singleton == null) return;
        if (NetworkManager.Singleton.ConnectedClientsList == null) return;
        var connectedClients = NetworkManager.Singleton.ConnectedClientsList;

        foreach (var checker in _checkers)
        {
            if (checker == null || !checker.IsSpawned) continue;

            // Lấy vị trí của owner object này
            Vector3 ownerPos = checker.transform.position;

            foreach (var client in connectedClients)
            {
                // Không ẩn chính owner của object đó
                if (client.ClientId == checker.OwnerClientId) continue;

                // Lấy player object của client đó
                var clientPlayerObj = client.PlayerObject;
                if (clientPlayerObj == null) continue;

                float dist = Vector3.Distance(ownerPos, clientPlayerObj.transform.position);
                float maxDist = checker.maxDistance > 0 ? checker.maxDistance : defaultMaxDistance;

                var networkObj = checker.GetComponent<NetworkObject>();
                if (networkObj == null) continue;

                bool isVisible = networkObj.IsNetworkVisibleTo(client.ClientId);

                if (dist > maxDist && isVisible)
                {
                    // Ẩn: xóa khỏi client này
                    networkObj.NetworkHide(client.ClientId);
                }
                else if (dist <= maxDist && !isVisible)
                {
                    // Hiện lại
                    networkObj.NetworkShow(client.ClientId);
                }
            }
        }
    }
}