using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[DisallowMultipleComponent]
public class OccluderFadable : TGTHMonoBehaviour
{
    [Header("Fade")]
    [Range(0f, 1f)] public float fadedAlpha = 0.25f;
    public float fadeSpeed = 8f;

    [Header("Renderers")]
    public Renderer[] renderers;

    float _current = 1f;
    float _target = 1f;

    static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    static readonly int ColorId     = Shader.PropertyToID("_Color");
    static readonly int SurfaceId   = Shader.PropertyToID("_Surface");
    static readonly int SrcBlendId  = Shader.PropertyToID("_SrcBlend");
    static readonly int DstBlendId  = Shader.PropertyToID("_DstBlend");
    static readonly int ZWriteId    = Shader.PropertyToID("_ZWrite");

    Material[][] _originalSharedMaterials;
    Material[][] _instancedMaterials;
    Color[][]    _originalColors;

    // Cache lại các giá trị opaque gốc để restore đúng
    float[][]    _originalSurface;
    int[][]      _originalSrcBlend;
    int[][]      _originalDstBlend;
    int[][]      _originalZWrite;
    int[][]      _originalRenderQueue;

    readonly List<Material> _created = new();
    bool _occluding = false;
    bool _isTransparentMode = false;

    protected override void LoadComponent()
    {
        base.LoadComponent();
        if (renderers == null || renderers.Length == 0)
            renderers = GetComponentsInChildren<Renderer>(true);
    }

    protected override void Awake()
    {
        base.Awake();

        if (renderers == null || renderers.Length == 0)
            renderers = GetComponentsInChildren<Renderer>(true);

        CreateMaterialInstances();
    }

    void OnDestroy()
    {
        RestoreOriginalMaterials();
        DestroyCreatedMaterials();
    }

    public void SetOccluding(bool occluding)
    {
        if (_occluding == occluding) return;
        _occluding = occluding;
        _target = _occluding ? fadedAlpha : 1f;

        // Bắt đầu fade OUT → chuyển sang transparent mode ngay
        if (_occluding)
            SetTransparentMode(true);

        enabled = true;
    }

    void Update()
    {
        _current = Mathf.MoveTowards(_current, _target, fadeSpeed * Time.deltaTime);
        ApplyAlphaToMaterials(_current);

        if (Mathf.Abs(_current - _target) < 0.001f)
        {
            _current = _target;
            ApplyAlphaToMaterials(_current);

            // Khi restore xong về alpha=1 → trả về opaque để tránh xuyên thấu
            if (_target >= 0.999f)
            {
                SetTransparentMode(false);
                enabled = false;
            }
        }
    }

    // ─── Tạo instance materials, cache giá trị gốc, KHÔNG đổi blend mode ───
    void CreateMaterialInstances()
    {
        int rCount = renderers.Length;

        _originalSharedMaterials = new Material[rCount][];
        _instancedMaterials      = new Material[rCount][];
        _originalColors          = new Color[rCount][];
        _originalSurface         = new float[rCount][];
        _originalSrcBlend        = new int[rCount][];
        _originalDstBlend        = new int[rCount][];
        _originalZWrite          = new int[rCount][];
        _originalRenderQueue     = new int[rCount][];

        for (int i = 0; i < rCount; i++)
        {
            var r = renderers[i];
            if (!r) continue;

            var shared = r.sharedMaterials;
            _originalSharedMaterials[i] = shared;

            int mCount = shared.Length;
            var inst    = new Material[mCount];
            var cols    = new Color[mCount];
            var surfs   = new float[mCount];
            var srcs    = new int[mCount];
            var dsts    = new int[mCount];
            var zws     = new int[mCount];
            var queues  = new int[mCount];

            for (int m = 0; m < mCount; m++)
            {
                var sm = shared[m];
                if (!sm) { inst[m] = null; cols[m] = Color.white; continue; }

                var im = new Material(sm);
                _created.Add(im);

                // Cache màu gốc
                if (im.HasProperty(BaseColorId))      cols[m] = im.GetColor(BaseColorId);
                else if (im.HasProperty(ColorId))     cols[m] = im.GetColor(ColorId);
                else                                  cols[m] = Color.white;

                // Cache blend state gốc
                surfs[m]  = im.HasProperty(SurfaceId)  ? im.GetFloat(SurfaceId)  : 0f;
                srcs[m]   = im.HasProperty(SrcBlendId) ? im.GetInt(SrcBlendId)   : (int)BlendMode.One;
                dsts[m]   = im.HasProperty(DstBlendId) ? im.GetInt(DstBlendId)   : (int)BlendMode.Zero;
                zws[m]    = im.HasProperty(ZWriteId)   ? im.GetInt(ZWriteId)     : 1;
                queues[m] = im.renderQueue;

                inst[m] = im;
            }

            _instancedMaterials[i]  = inst;
            _originalColors[i]      = cols;
            _originalSurface[i]     = surfs;
            _originalSrcBlend[i]    = srcs;
            _originalDstBlend[i]    = dsts;
            _originalZWrite[i]      = zws;
            _originalRenderQueue[i] = queues;

            r.materials = inst;
        }
    }

    // ─── Chuyển đổi giữa Opaque ↔ Transparent ───
    void SetTransparentMode(bool transparent)
    {
        if (_isTransparentMode == transparent) return;
        _isTransparentMode = transparent;

        for (int i = 0; i < renderers.Length; i++)
        {
            var mats = _instancedMaterials[i];
            if (mats == null) continue;

            for (int m = 0; m < mats.Length; m++)
            {
                var mat = mats[m];
                if (!mat) continue;

                if (transparent)
                {
                    // → Transparent
                    if (mat.HasProperty(SurfaceId))   mat.SetFloat(SurfaceId, 1f);
                    if (mat.HasProperty(SrcBlendId))  mat.SetInt(SrcBlendId, (int)BlendMode.SrcAlpha);
                    if (mat.HasProperty(DstBlendId))  mat.SetInt(DstBlendId, (int)BlendMode.OneMinusSrcAlpha);
                    if (mat.HasProperty(ZWriteId))    mat.SetInt(ZWriteId, 0);
                    mat.renderQueue = (int)RenderQueue.Transparent;
                    mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                }
                else
                {
                    // → Restore về opaque gốc
                    if (mat.HasProperty(SurfaceId))   mat.SetFloat(SurfaceId, _originalSurface[i][m]);
                    if (mat.HasProperty(SrcBlendId))  mat.SetInt(SrcBlendId, _originalSrcBlend[i][m]);
                    if (mat.HasProperty(DstBlendId))  mat.SetInt(DstBlendId, _originalDstBlend[i][m]);
                    if (mat.HasProperty(ZWriteId))    mat.SetInt(ZWriteId, _originalZWrite[i][m]);
                    mat.renderQueue = _originalRenderQueue[i][m];
                    mat.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");

                    // Reset alpha về 1 cho chắc
                    var c = _originalColors[i][m];
                    c.a = 1f;
                    if (mat.HasProperty(BaseColorId))     mat.SetColor(BaseColorId, c);
                    else if (mat.HasProperty(ColorId))    mat.SetColor(ColorId, c);
                }
            }
        }
    }

    void ApplyAlphaToMaterials(float a)
    {
        if (_instancedMaterials == null) return;

        for (int i = 0; i < renderers.Length; i++)
        {
            var r = renderers[i];
            if (!r) continue;

            var mats = _instancedMaterials[i];
            var cols = _originalColors[i];
            if (mats == null || cols == null) continue;

            for (int m = 0; m < mats.Length; m++)
            {
                var mat = mats[m];
                if (!mat) continue;

                var c = cols[m];
                c.a = a;

                if (mat.HasProperty(BaseColorId))     mat.SetColor(BaseColorId, c);
                else if (mat.HasProperty(ColorId))    mat.SetColor(ColorId, c);
            }
        }
    }

    void RestoreOriginalMaterials()
    {
        if (_originalSharedMaterials == null || renderers == null) return;

        for (int i = 0; i < renderers.Length; i++)
        {
            var r = renderers[i];
            if (!r) continue;

            var shared = _originalSharedMaterials[i];
            if (shared != null) r.sharedMaterials = shared;
        }
    }

    void DestroyCreatedMaterials()
    {
        foreach (var m in _created)
            if (m) Destroy(m);
        _created.Clear();
    }
}