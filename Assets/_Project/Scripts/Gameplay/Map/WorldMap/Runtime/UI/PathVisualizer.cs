using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PathVisualizer : Singleton<PathVisualizer>
{
    [Header("Visible")]
    [SerializeField] private bool isVisible = true;

    [Header("Dot Settings")]
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private float spacing = 0.5f;
    [SerializeField] private float yOffset = 0.1f;

    [Header("Player")]
    [SerializeField] private Transform player;
    [SerializeField] private float hideDistance = 0.4f;
    [SerializeField] private float maxDistanceFromPath = 5f;

    private readonly List<GameObject> dots = new();

    protected override void Awake()
    {
        base.Awake();

        if (PlayerNetManager.Instance != null)
            PlayerNetManager.Instance.OnPlayerExiststed += OnPlayerExiststed;
    }

    private void OnDestroy()
    {
        if (PlayerNetManager.Instance != null)
            PlayerNetManager.Instance.OnPlayerExiststed -= OnPlayerExiststed;
    }

    private void OnPlayerExiststed(NetworkObject playerNet)
    {
        if (playerNet == null)
            return;

        player = playerNet.transform;
    }

    private void Update()
    {
        if (!isVisible)
            return;

        CheckTooFarFromPath();
        HidePassedDots();
    }

    public void Draw(List<Vector3> corners)
    {
        Clear();

        if (!isVisible)
            return;

        if (dotPrefab == null)
        {
            Debug.LogError("PathVisualizer: dotPrefab chưa được gán.");
            return;
        }

        if (corners == null || corners.Count < 2)
            return;

        for (int i = 0; i < corners.Count - 1; i++)
        {
            bool includeStartDot = i == 0;
            DrawDotsBetween(corners[i], corners[i + 1], includeStartDot);
        }
    }

    private void DrawDotsBetween(Vector3 start, Vector3 end, bool includeStartDot)
    {
        float distance = Vector3.Distance(start, end);

        if (distance <= 0.01f)
            return;

        int count = Mathf.Max(1, Mathf.CeilToInt(distance / spacing));

        for (int i = 0; i <= count; i++)
        {
            if (!includeStartDot && i == 0)
                continue;

            float t = i / (float)count;

            Vector3 pos = Vector3.Lerp(start, end, t);
            pos.y += yOffset;

            GameObject dot = ObjectPool.Instance.GetObject(
                dotPrefab,
                pos,
                Quaternion.identity
            );

            dots.Add(dot);
        }
    }

    private void HidePassedDots()
    {
        if (player == null || dots.Count == 0)
            return;

        int passedIndex = -1;

        for (int i = 0; i < dots.Count; i++)
        {
            GameObject dot = dots[i];

            if (dot == null)
                continue;

            float distance = DistanceXZ(player.position, dot.transform.position);

            if (distance <= hideDistance)
                passedIndex = i;
        }

        if (passedIndex >= 0)
            RemoveDotsUntil(passedIndex);
    }

    private void CheckTooFarFromPath()
    {
        if (player == null || dots.Count == 0)
            return;

        float closestDistance = float.MaxValue;

        for (int i = 0; i < dots.Count; i++)
        {
            GameObject dot = dots[i];

            if (dot == null)
                continue;

            float distance = DistanceXZ(player.position, dot.transform.position);

            if (distance < closestDistance)
                closestDistance = distance;
        }

        if (closestDistance > maxDistanceFromPath)
            Clear();
    }

    private void RemoveDotsUntil(int index)
    {
        index = Mathf.Clamp(index, 0, dots.Count - 1);

        for (int i = 0; i <= index; i++)
        {
            if (dots[i] != null)
                ObjectPool.Instance.ReturnObject(dots[i]);
        }

        dots.RemoveRange(0, index + 1);
    }

    private float DistanceXZ(Vector3 a, Vector3 b)
    {
        a.y = 0;
        b.y = 0;
        return Vector3.Distance(a, b);
    }

    public void Clear()
    {
        for (int i = dots.Count - 1; i >= 0; i--)
        {
            if (dots[i] != null)
                ObjectPool.Instance.ReturnObject(dots[i]);
        }

        dots.Clear();
    }

    public void SetVisible(bool value)
    {
        isVisible = value;

        if (!isVisible)
            Clear();
    }

    public void SetPlayer(Transform target)
    {
        player = target;
    }
}