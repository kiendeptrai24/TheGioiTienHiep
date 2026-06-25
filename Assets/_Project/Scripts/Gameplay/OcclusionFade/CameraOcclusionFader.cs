using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CameraOcclusionFader : Singleton<CameraOcclusionFader>
{
#if !UNITY_SERVER
    [Header("Refs")]
    public Transform target;
    public Transform camPivotOrCamera;

    [Header("Cast")]
    public LayerMask occluderMask = ~0;
    public float sphereRadius = 0.25f;
    public float extraDistance = 0.2f;
    public QueryTriggerInteraction triggerMode = QueryTriggerInteraction.Ignore;

    [Header("Ignore Target Colliders")]
    public bool ignoreTargetColliders = true;

    [Header("Debug")]
    public bool drawDebug;

    public event Action<bool> OnOccluded;

    readonly HashSet<OccluderFadable> _current = new();
    readonly HashSet<OccluderFadable> _next = new();
    readonly HashSet<Collider> _ignore = new();
    readonly Dictionary<Collider, OccluderFadable> _fadableCache = new();

    RaycastHit[] _hits = new RaycastHit[64];
    bool _isOccluded;

    protected override void LoadComponent()
    {
        base.LoadComponent();

        if (camPivotOrCamera) return;

        var cam = GetComponentInChildren<Camera>();
        if (cam) camPivotOrCamera = cam.transform;
    }

    protected override void Awake()
    {
        base.Awake();

        if (PlayerNetManager.Instance != null)
            PlayerNetManager.Instance.OnPlayerExiststed += OnPlayerExists;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (PlayerNetManager.Instance != null)
            PlayerNetManager.Instance.OnPlayerExiststed -= OnPlayerExists;
    }

    void OnPlayerExists(NetworkObject obj)
    {
        if (!obj) return;

        target = obj.transform;
        RebuildIgnoreList();
    }

    void RebuildIgnoreList()
    {
        _ignore.Clear();

        if (!ignoreTargetColliders || !target) return;

        var colliders = target.GetComponentsInChildren<Collider>(true);
        for (int i = 0; i < colliders.Length; i++)
            if (colliders[i]) _ignore.Add(colliders[i]);
    }

    void LateUpdate()
    {
        if (!target || !camPivotOrCamera) return;

        Vector3 cameraPosition = camPivotOrCamera.position;
        Vector3 targetPosition = target.position;
        Vector3 direction = targetPosition - cameraPosition;
        float distance = direction.magnitude;
        if (distance <= 0.0001f) return;

        direction /= distance;

        Vector3 origin = cameraPosition + direction * 0.05f;
        float castDistance = distance + extraDistance;

        int hitCount = Physics.SphereCastNonAlloc(
            origin,
            sphereRadius,
            direction,
            _hits,
            castDistance,
            occluderMask,
            triggerMode
        );

        _next.Clear();

        for (int i = 0; i < hitCount; i++)
        {
            var colliderHit = _hits[i].collider;
            if (!colliderHit || _ignore.Contains(colliderHit)) continue;

            if (!_fadableCache.TryGetValue(colliderHit, out var fadable))
            {
                fadable = colliderHit.GetComponentInParent<OccluderFadable>();
                _fadableCache[colliderHit] = fadable;
            }

            if (fadable) _next.Add(fadable);
        }

        foreach (var fadable in _next)
        {
            _current.Add(fadable);
            fadable.SetOccluding(true);
        }

        if (_current.Count > 0)
        {
            var toRestore = ListPool<OccluderFadable>.Get();

            foreach (var fadable in _current)
                if (!_next.Contains(fadable))
                    toRestore.Add(fadable);

            foreach (var fadable in toRestore)
            {
                _current.Remove(fadable);
                if (fadable) fadable.SetOccluding(false);
            }

            ListPool<OccluderFadable>.Release(toRestore);
        }

        bool nowOccluded = _next.Count > 0;
        if (nowOccluded != _isOccluded)
        {
            _isOccluded = nowOccluded;
            OnOccluded?.Invoke(_isOccluded);
        }

        if (drawDebug)
            Debug.DrawLine(origin, origin + direction * distance, Color.yellow);
    }

    static class ListPool<T>
    {
        static readonly Stack<List<T>> Pool = new();

        public static List<T> Get() => Pool.Count > 0 ? Pool.Pop() : new List<T>(16);

        public static void Release(List<T> list)
        {
            list.Clear();
            Pool.Push(list);
        }
    }
#endif
}
