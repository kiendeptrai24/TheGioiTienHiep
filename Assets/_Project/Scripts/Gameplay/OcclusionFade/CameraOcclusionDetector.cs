using System;
using UnityEngine;

namespace TheGioiTienHiep.Gameplay.OcclusionFade
{
    /// <summary>
    /// Kiểm tra đường nhìn từ camera tới player bằng SphereCastNonAlloc / RaycastNonAlloc.
    /// Gắn vào cùng GameObject với PlayerVisibilityTintURP.
    /// Chỉ bật component này cho owner player — <see cref="PlayerVisibilityTintURP"/> xử lý việc đó.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class CameraOcclusionDetector : MonoBehaviour
    {
        // ─────────────────────────────────────────────────────────────────────
        // Inspector
        // ─────────────────────────────────────────────────────────────────────

        [Header("Cast Settings")]
        [Tooltip("Layer mask xác định những vật thể nào được coi là occluder (vật chắn).")]
        [SerializeField] private LayerMask _occluderMask = -1;

        [Tooltip("Khoảng thời gian (giây) giữa các lần cast. Tăng lên để tiết kiệm CPU mobile.")]
        [SerializeField, Range(0.05f, 1f)] private float _checkInterval = 0.1f;

        [Tooltip("Bán kính SphereCast. Đặt = 0 để dùng RaycastNonAlloc (nhanh hơn, ít chính xác hơn).")]
        [SerializeField, Range(0f, 1f)] private float _castRadius = 0.25f;

        [Tooltip("Kích thước buffer hit pre-allocated. Không cần đặt quá lớn.")]
        [SerializeField, Range(1, 16)] private int _hitBufferSize = 8;

        [Header("Target Point")]
        [Tooltip("Offset từ gốc transform của player để làm điểm nhắm (ví dụ: ngực nhân vật).")]
        [SerializeField] private Vector3 _targetOffset = new Vector3(0f, 1.2f, 0f);

        // ─────────────────────────────────────────────────────────────────────
        // Public API
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Camera dùng để cast. Được gán bởi <see cref="PlayerVisibilityTintURP"/>.</summary>
        public Camera TargetCamera { get; set; }

        /// <summary>
        /// Bắn event khi trạng thái bị che thay đổi.
        /// <c>true</c> = bị occlude; <c>false</c> = nhìn thấy bình thường.
        /// </summary>
        public event Action<bool> OnOcclusionChanged;

        /// <summary>Trạng thái hiện tại — đọc từ bên ngoài không cần đăng ký event.</summary>
        public bool IsOccluded { get; private set; }

        // ─────────────────────────────────────────────────────────────────────
        // Private state
        // ─────────────────────────────────────────────────────────────────────

        private RaycastHit[] _hitBuffer;
        private float _nextCheckTime;

        // ─────────────────────────────────────────────────────────────────────
        // Lifecycle
        // ─────────────────────────────────────────────────────────────────────

        private void Awake()
        {
            _hitBuffer = new RaycastHit[_hitBufferSize];
        }

        private void OnEnable()
        {
            // Reset timer khi enable để check ngay lập tức trong frame đầu.
            _nextCheckTime = 0f;
        }

        private void Update()
        {
            if (TargetCamera == null) return;
            if (Time.time < _nextCheckTime) return;

            _nextCheckTime = Time.time + _checkInterval;
            PerformCheck();
        }

        private void OnDisable()
        {
            // Khi component bị tắt (ví dụ: chuyển scene, character die)
            // đảm bảo player không bị kẹt trong trạng thái X-Ray.
            if (IsOccluded)
                SetOcclusionState(false);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Detection
        // ─────────────────────────────────────────────────────────────────────

        private void PerformCheck()
        {
            Vector3 origin = TargetCamera.transform.position;
            Vector3 target = transform.position + _targetOffset;
            Vector3 direction = target - origin;
            float distance = direction.magnitude;

            if (distance < 0.01f)
            {
                SetOcclusionState(false);
                return;
            }

            // Normalize in-place để tránh tạo Vector3 mới
            float invDist = 1f / distance;
            direction.x *= invDist;
            direction.y *= invDist;
            direction.z *= invDist;

            int hitCount = _castRadius > 0f
                ? Physics.SphereCastNonAlloc(origin, _castRadius, direction, _hitBuffer,
                      distance, _occluderMask, QueryTriggerInteraction.Ignore)
                : Physics.RaycastNonAlloc(origin, direction, _hitBuffer,
                      distance, _occluderMask, QueryTriggerInteraction.Ignore);

            bool occluded = false;
            Transform root = transform.root;

            for (int i = 0; i < hitCount; i++)
            {
                // Loại bỏ collider của chính player (kể cả child collider)
                if (_hitBuffer[i].transform.root != root)
                {
                    occluded = true;
                    break;
                }
            }

            SetOcclusionState(occluded);
        }

        private void SetOcclusionState(bool newState)
        {
            if (IsOccluded == newState) return;
            IsOccluded = newState;
            OnOcclusionChanged?.Invoke(IsOccluded);
        }
    }
}
