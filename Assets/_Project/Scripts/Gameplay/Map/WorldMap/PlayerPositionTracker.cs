using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerPositionTracker : Singleton<PlayerPositionTracker>
{
    public event Action<int, int> OnPositionChanged;

    private Vector3 lastPosition;
    private int xPos;
    private int zPos;
    private Transform playerPos;
    protected override void Awake()
    {
        base.Awake();
        PlayerNetManager.Instance.OnPlayerExiststed += OnPlayerExiststed;
    }

    private void OnPlayerExiststed(NetworkObject @object)
    {
        if (playerPos == null) playerPos = @object.transform;
    }
    private void Update()
    {
        if (playerPos != null)
        {
            int x = (int)playerPos.position.x;
            int z = (int)playerPos.position.z;
            if (xPos == x && zPos == z) return;
            xPos = x;
            zPos = z;
            OnPositionChanged?.Invoke(x, z);
        }
    }
}