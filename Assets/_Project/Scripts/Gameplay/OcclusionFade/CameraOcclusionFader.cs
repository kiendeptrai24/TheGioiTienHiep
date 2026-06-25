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
    [Min(0f)] public float castInterval = 0.02f;
    [Min(0f)] public float recastPositionThreshold = 0.02f;
    [Min(0f)] public float staticRecastInterval = 0.1f;

    [Header("Ignore Target Colliders")]
    public bool ignoreTargetColliders = true;

    [Header("Debug")]
    public bool drawDebug;

    public event Action<bool> OnOccluded;

    readonly HashSet<OccluderFadable> _current = new();
    readonly HashSet<OccluderFadable> _next = new();
    readonly HashSet<Collider> _ignore = new();
    readonly Dictionary<Collider, OccluderFadable> _fadableCache = new();
    readonly List<OccluderFadable> _toRestore = new(16);

    RaycastHit[] _hits = new RaycastHit[64];
    bool _isOccluded;
    private bool canOcclude = false;
    float _nextCastTime;
    float _nextStaticCastTime;
    float _recastPositionThresholdSqr;
    Vector3 _lastCameraPosition;
    Vector3 _lastTargetPosition;
    bool _hasLastSample;

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
        _recastPositionThresholdSqr = recastPositionThreshold * recastPositionThreshold;

        if (PlayerNetManager.Instance != null)
            PlayerNetManager.Instance.OnPlayerExiststed += OnPlayerExists;
        SetOccluded(false);
    }

    protected void OnValidate()
    {
        _recastPositionThresholdSqr = recastPositionThreshold * recastPositionThreshold;
    }

    protected void OnDestroy()
    {
        if (PlayerNetManager.Instance != null)
            PlayerNetManager.Instance.OnPlayerExiststed -= OnPlayerExists;
    }

    void OnPlayerExists(NetworkObject obj)
    {
        if (!obj) return;

        target = obj.transform;
        _hasLastSample = false;
        _nextCastTime = 0f;
        _nextStaticCastTime = 0f;
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

        if (!ShouldRecast(cameraPosition, targetPosition))
        {
            if (drawDebug)
                DebugDraw(cameraPosition, targetPosition);
            return;
        }

        ProcessOcclusion(cameraPosition, targetPosition);
    }

    bool ShouldRecast(Vector3 cameraPosition, Vector3 targetPosition)
    {
        if (!_hasLastSample) return true;

        float now = Time.unscaledTime;
        bool castIntervalElapsed = castInterval <= 0f || now >= _nextCastTime;
        bool staticIntervalElapsed = staticRecastInterval <= 0f || now >= _nextStaticCastTime;
        if (!castIntervalElapsed) return false;

        if (_recastPositionThresholdSqr <= 0f) return true;

        bool cameraMoved = (cameraPosition - _lastCameraPosition).sqrMagnitude >= _recastPositionThresholdSqr;
        bool targetMoved = (targetPosition - _lastTargetPosition).sqrMagnitude >= _recastPositionThresholdSqr;
        return cameraMoved || targetMoved || staticIntervalElapsed;
    }

    void ProcessOcclusion(Vector3 cameraPosition, Vector3 targetPosition)
    {
        _hasLastSample = true;
        _lastCameraPosition = cameraPosition;
        _lastTargetPosition = targetPosition;
        _nextCastTime = Time.unscaledTime + castInterval;
        _nextStaticCastTime = Time.unscaledTime + staticRecastInterval;

        Vector3 direction = targetPosition - cameraPosition;
        float distance = direction.magnitude;
        if (distance <= 0.0001f)
        {
            RestoreAll();
            return;
        }

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

        if (hitCount == _hits.Length)
            Array.Resize(ref _hits, _hits.Length * 2);

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
            _toRestore.Clear();

            foreach (var fadable in _current)
                if (!_next.Contains(fadable))
                    _toRestore.Add(fadable);

            for (int i = 0; i < _toRestore.Count; i++)
            {
                var fadable = _toRestore[i];
                _current.Remove(fadable);
                if (fadable) fadable.SetOccluding(false);
            }
        }

        UpdateOccludedState();

        if (drawDebug)
            Debug.DrawLine(origin, origin + direction * distance, Color.yellow);
    }

    void RestoreAll()
    {
        if (_current.Count > 0)
        {
            _toRestore.Clear();
            foreach (var fadable in _current)
                _toRestore.Add(fadable);

            _current.Clear();

            for (int i = 0; i < _toRestore.Count; i++)
            {
                var fadable = _toRestore[i];
                if (fadable) fadable.SetOccluding(false);
            }
        }

        UpdateOccludedState();
    }

    void UpdateOccludedState()
    {
        bool nowOccluded = _current.Count > 0;
        if (nowOccluded == _isOccluded) return;

        _isOccluded = nowOccluded;
        if (canOcclude)
            OnOccluded?.Invoke(_isOccluded);
    }
    public void SetOccluded(bool occluded)
    {
        canOcclude = occluded;
    }

    void DebugDraw(Vector3 cameraPosition, Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - cameraPosition;
        float distance = direction.magnitude;
        if (distance <= 0.0001f) return;

        direction /= distance;
        Vector3 origin = cameraPosition + direction * 0.05f;
        Debug.DrawLine(origin, origin + direction * distance, Color.yellow);
    }
#endif
}
