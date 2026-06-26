using UnityEngine;
using Unity.Netcode;

namespace TheGioiTienHiep.Gameplay.OcclusionFade
{
    /// <summary>
    /// Nhận event từ <see cref="CameraOcclusionDetector"/> và bật/tắt hiệu ứng X-Ray URP.
    ///
    /// ═══════════════════════════════════════════════════════════════════════
    ///  HƯỚNG DẪN SETUP SHADER / MATERIAL (chọn 1 trong 2 phương án)
    /// ═══════════════════════════════════════════════════════════════════════
    ///
    ///  PHƯƠNG ÁN A — Material đơn giản ZTest Always (khuyến nghị mobile)
    ///  ─────────────────────────────────────────────────────────────────────
    ///  1. Tạo material "M_PlayerXRay" dùng shader "Custom/PlayerXRay"
    ///     (file PlayerXRay.shader cùng thư mục này).
    ///  2. Chỉnh màu _Color (ví dụ: cyan 50% alpha) và _RimPower (2–3).
    ///  3. Gán material vào trường XRayMaterial trên component này.
    ///  Kết quả: Khi player bị che, toàn bộ character đổi sang silhouette
    ///  bán trong suốt, nhìn thấy xuyên tường.
    ///
    ///  PHƯƠNG ÁN B — URP Renderer Feature (hiệu ứng chính xác kiểu AFK Journey)
    ///  ─────────────────────────────────────────────────────────────────────
    ///  Character vẫn hiện bình thường VÀ đồng thời có silhouette xuyên tường.
    ///
    ///  1. Vào Project Settings > Tags & Layers → thêm layer mới tên "XRay".
    ///  2. Mở URP Renderer asset (thường ở Settings/URP/…Renderer.asset).
    ///  3. Add Renderer Feature → "Render Objects".
    ///     • Name: "Player XRay Pass"
    ///     • Layer Mask: chọn "XRay"
    ///     • Event: "After Rendering Opaques"
    ///     • Override Depth State: ✓
    ///       - Depth Write: Off
    ///       - Depth Test: Greater  ← chỉ vẽ phần BỊ CHẶN bởi wall
    ///     • (Tuỳ chọn) Override Material: gán M_PlayerXRay để đổi màu silhouette
    ///       nếu không muốn dùng material gốc.
    ///  4. Bật "Use Layer Switch" trên component này và điền "XRay" vào XRay Layer Name.
    ///  5. KHÔNG gán XRayMaterial khi dùng phương án B (hoặc gán material trong suốt).
    ///  Kết quả: Layer switch cho phép Renderer Feature vẽ thêm một pass,
    ///  character hiện bình thường + phần sau tường hiện silhouette màu.
    ///
    ///  LƯU Ý NETCODE:
    ///  Component này kế thừa NetworkBehaviour. Chỉ owner mới khởi tạo
    ///  hệ thống và bật CameraOcclusionDetector. Non-owner sẽ bị skip.
    ///  Nếu không dùng Netcode, đảm bảo KHÔNG có NetworkObject trên prefab
    ///  hoặc NetworkManager không có trong scene — Start() sẽ tự khởi tạo.
    /// ═══════════════════════════════════════════════════════════════════════
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CameraOcclusionDetector))]
    public sealed class PlayerVisibilityTintURP : NetworkBehaviour
    {
        // ─────────────────────────────────────────────────────────────────────
        // Inspector
        // ─────────────────────────────────────────────────────────────────────

        [Header("Phương án A — Material X-Ray")]
        [Tooltip("Material ZTest Always PRE-CREATED (KHÔNG tạo mới runtime). " +
                 "Dùng shader Custom/PlayerXRay hoặc URP Unlit với ZTest Always.")]
        [SerializeField] private Material _xRayMaterial;

        [Header("Renderers (để trống → tự tìm)")]
        [Tooltip("Danh sách Renderer cần đổi material. Nếu để trống sẽ dùng GetComponentsInChildren.")]
        [SerializeField] private Renderer[] _renderers;

        [Header("Phương án B — Layer Switch")]
        [Tooltip("Bật khi dùng URP Renderer Feature Render Objects thay vì đổi material.")]
        [SerializeField] private bool _useLayerSwitch = false;

        [Tooltip("Tên layer dành riêng cho Render Objects pass. Tạo layer này trong Tags & Layers.")]
        [SerializeField] private string _xRayLayerName = "XRay";

        // ─────────────────────────────────────────────────────────────────────
        // Cache — pre-allocated, KHÔNG new Material() tại runtime
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>sharedMaterials gốc của từng renderer, lưu ngay lúc init.</summary>
        private Material[][] _originalSharedMaterials;

        /// <summary>
        /// Mảng material X-Ray pre-built cho từng renderer (cùng số slot với original).
        /// Tất cả slot trỏ về <see cref="_xRayMaterial"/> — không cấp phát runtime.
        /// </summary>
        private Material[][] _xRayMaterialArrays;

        /// <summary>Layer gốc của từng renderer GameObject (dùng cho phương án B).</summary>
        private int[] _originalLayers;

        private int _xRayLayerIndex = -1;

        // ─────────────────────────────────────────────────────────────────────
        // State
        // ─────────────────────────────────────────────────────────────────────

        private CameraOcclusionDetector _detector;
        private bool _isXRayActive;
        private bool _initialized;
        private bool _cleanedUp;

        // ─────────────────────────────────────────────────────────────────────
        // Netcode lifecycle
        // ─────────────────────────────────────────────────────────────────────

        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
            {
                // Non-owner: tắt hoàn toàn detector để không waste CPU
                var det = GetComponent<CameraOcclusionDetector>();
                if (det != null) det.enabled = false;
                return;
            }

            InitializeSystem();
        }

        public override void OnNetworkDespawn()
        {
            Cleanup();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Offline / non-networked fallback
        // ─────────────────────────────────────────────────────────────────────

        private void Start()
        {
            // Chỉ init ở đây nếu không có NetworkManager (non-networked scene).
            // Nếu có NetworkManager thì OnNetworkSpawn xử lý.
            if (NetworkManager.Singleton == null)
                InitializeSystem();
        }

        private void OnDestroy()
        {
            Cleanup();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Initialization
        // ─────────────────────────────────────────────────────────────────────

        private void InitializeSystem()
        {
            if (_initialized) return;

            if (!ValidateSetup()) return;

            CacheRenderers();
            CacheLayers();
            SetupDetector();

            _initialized = true;
        }

        private bool ValidateSetup()
        {
            // Phương án A: cần _xRayMaterial
            if (!_useLayerSwitch && _xRayMaterial == null)
            {
                Debug.LogError(
                    $"[PlayerVisibilityTintURP] XRayMaterial chưa được gán trên '{name}'. " +
                    "Gán material hoặc bật Use Layer Switch để dùng phương án B.", this);
                return false;
            }
            return true;
        }

        private void CacheRenderers()
        {
            if (_renderers == null || _renderers.Length == 0)
                _renderers = GetComponentsInChildren<Renderer>(true);

            _originalSharedMaterials = new Material[_renderers.Length][];
            _xRayMaterialArrays = new Material[_renderers.Length][];

            for (int i = 0; i < _renderers.Length; i++)
            {
                // Cache mảng sharedMaterials gốc — KHÔNG gọi .materials (tránh tạo instance)
                _originalSharedMaterials[i] = _renderers[i].sharedMaterials;

                // Pre-build mảng X-Ray — tái sử dụng cùng 1 material object cho tất cả slot
                if (_xRayMaterial != null)
                {
                    int slotCount = _originalSharedMaterials[i].Length;
                    _xRayMaterialArrays[i] = new Material[slotCount];
                    for (int j = 0; j < slotCount; j++)
                        _xRayMaterialArrays[i][j] = _xRayMaterial;
                }
            }
        }

        private void CacheLayers()
        {
            if (!_useLayerSwitch) return;

            _xRayLayerIndex = LayerMask.NameToLayer(_xRayLayerName);
            if (_xRayLayerIndex < 0)
            {
                Debug.LogWarning(
                    $"[PlayerVisibilityTintURP] Layer '{_xRayLayerName}' không tồn tại. " +
                    "Tạo layer này tại Project Settings > Tags & Layers. Layer switch bị tắt.", this);
                _useLayerSwitch = false;
                return;
            }

            _originalLayers = new int[_renderers.Length];
            for (int i = 0; i < _renderers.Length; i++)
                _originalLayers[i] = _renderers[i].gameObject.layer;
        }

        private void SetupDetector()
        {
            _detector = GetComponent<CameraOcclusionDetector>();
            if (_detector == null)
            {
                Debug.LogWarning(
                    $"[PlayerVisibilityTintURP] Không tìm thấy CameraOcclusionDetector trên '{name}'.", this);
                return;
            }

            _detector.TargetCamera = Camera.main;
            _detector.enabled = true;
            _detector.OnOcclusionChanged += HandleOcclusionChanged;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Occlusion response — chỉ đổi material KHI STATE THAY ĐỔI, không mỗi frame
        // ─────────────────────────────────────────────────────────────────────

        private void HandleOcclusionChanged(bool isOccluded)
        {
            if (_isXRayActive == isOccluded) return;
            _isXRayActive = isOccluded;

            if (_isXRayActive)
                ApplyXRay();
            else
                RestoreOriginal();
        }

        private void ApplyXRay()
        {
            for (int i = 0; i < _renderers.Length; i++)
            {
                if (_renderers[i] == null) continue;

                // Phương án A: đổi toàn bộ sang X-Ray material
                if (_xRayMaterialArrays != null && _xRayMaterialArrays[i] != null)
                    _renderers[i].sharedMaterials = _xRayMaterialArrays[i];

                // Phương án B: chuyển sang XRay layer để Render Objects pass bắt
                if (_useLayerSwitch && _xRayLayerIndex >= 0)
                    _renderers[i].gameObject.layer = _xRayLayerIndex;
            }
        }

        private void RestoreOriginal()
        {
            for (int i = 0; i < _renderers.Length; i++)
            {
                if (_renderers[i] == null) continue;

                if (_originalSharedMaterials != null && _originalSharedMaterials[i] != null)
                    _renderers[i].sharedMaterials = _originalSharedMaterials[i];

                if (_useLayerSwitch && _originalLayers != null)
                    _renderers[i].gameObject.layer = _originalLayers[i];
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Cleanup
        // ─────────────────────────────────────────────────────────────────────

        private void Cleanup()
        {
            if (_cleanedUp) return;
            _cleanedUp = true;

            if (_detector != null)
                _detector.OnOcclusionChanged -= HandleOcclusionChanged;

            // Đảm bảo player không bị kẹt ở X-Ray khi despawn / destroy
            if (_isXRayActive && _initialized)
                RestoreOriginal();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Public API
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Cập nhật camera reference — gọi khi camera thay đổi (ví dụ: chuyển camera mode).
        /// </summary>
        public void SetCamera(Camera cam)
        {
            if (_detector != null)
                _detector.TargetCamera = cam;
        }
    }
}
