using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerPositionTracker : Singleton<PlayerPositionTracker>
{
    public event Action<int, int> OnPositionChanged;

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
        // Không có transform hoặc không ai lắng nghe -> thoát sớm, tránh tính toán vô ích
        if (playerPos == null || OnPositionChanged == null)
            return;

        // Cache position để chỉ gọi native interop 1 lần
        Vector3 pos = playerPos.position;
        int x = (int)pos.x;
        int z = (int)pos.z;

        if (xPos == x && zPos == z)
            return;

        xPos = x;
        zPos = z;

        // Dùng .Invoke thay vì ?.Invoke vì đã null-check ở trên
        OnPositionChanged.Invoke(x, z);
    }
}