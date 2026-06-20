using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CameraOcclusionFader : TGTHMonoBehaviour
{
#if !UNITY_SERVER
    [Header("Refs")]
    public Transform target;                 // player
    public Transform camPivotOrCamera;       // camera transform

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
    RaycastHit[] _hits = new RaycastHit[64];

    bool _isOccluded;

    protected override void LoadComponent()
    {
        base.LoadComponent();

        if (!camPivotOrCamera)
        {
            var cam = GetComponentInChildren<Camera>();
            if (cam) camPivotOrCamera = cam.transform;
        }
    }

    protected override void Awake()
    {
        base.Awake();

        // an toàn nếu Instance chưa có
        if (PlayerNetManager.Instance != null)
            PlayerNetManager.Instance.OnPlayerExiststed += OnPlayerExists;
    }

    protected void OnDestroy()
    {
        // TGTHMonoBehaviour có OnDestroy override thì giữ base nếu cần
        if (PlayerNetManager.Instance != null)
            PlayerNetManager.Instance.OnPlayerExiststed -= OnPlayerExists;

    }

    private void OnPlayerExists(NetworkObject obj)
    {
        if (!obj) return;

        target = obj.transform;
        RebuildIgnoreList();
    }

    void RebuildIgnoreList()
    {
        _ignore.Clear();
        if (!ignoreTargetColliders || !target) return;

        var cols = target.GetComponentsInChildren<Collider>(true);
        for (int i = 0; i < cols.Length; i++)
            if (cols[i]) _ignore.Add(cols[i]);
    }

    void LateUpdate()
    {
        if (!target || !camPivotOrCamera) return;

        Vector3 camPos = camPivotOrCamera.position;
        Vector3 tgtPos = target.position;

        Vector3 dir = (tgtPos - camPos);
        float dist = dir.magnitude;
        if (dist <= 0.0001f) return;

        dir /= dist;

        // tránh cast "dính" ngay tại camera
        Vector3 origin = camPos + dir * 0.05f;
        float castDist = dist + extraDistance;

        int hitCount = Physics.SphereCastNonAlloc(
            origin,
            sphereRadius,
            dir,
            _hits,
            castDist,
            occluderMask,
            triggerMode
        );

        _next.Clear();

        for (int i = 0; i < hitCount; i++)
        {
            var col = _hits[i].collider;
            if (!col) continue;

            // ignore collider của player
            if (_ignore.Contains(col)) continue;

            var fadable = col.GetComponentInParent<OccluderFadable>();
            if (!fadable) continue;

            _next.Add(fadable);
        }

        // Fade out vật đang che
        foreach (var f in _next)
        {
            _current.Add(f);
            f.SetOccluding(true);
        }

        // Fade in vật không còn che
        if (_current.Count > 0)
        {
            var toRestore = ListPool<OccluderFadable>.Get();

            foreach (var f in _current)
                if (!_next.Contains(f))
                    toRestore.Add(f);

            foreach (var f in toRestore)
            {
                _current.Remove(f);
                if (f) f.SetOccluding(false);
            }

            ListPool<OccluderFadable>.Release(toRestore);
        }

        // ✅ báo trạng thái occluded cho player tint/outline...
        bool nowOccluded = _next.Count > 0;
        if (nowOccluded != _isOccluded)
        {
            _isOccluded = nowOccluded;
            OnOccluded?.Invoke(_isOccluded);
        }

        if (drawDebug)
            Debug.DrawLine(origin, origin + dir * dist, Color.yellow);
    }

    // pool list nhỏ để tránh GC
    static class ListPool<T>
    {
        static readonly Stack<List<T>> Pool = new();
        public static List<T> Get() => Pool.Count > 0 ? Pool.Pop() : new List<T>(16);
        public static void Release(List<T> list) { list.Clear(); Pool.Push(list); }
    }
#endif
}
