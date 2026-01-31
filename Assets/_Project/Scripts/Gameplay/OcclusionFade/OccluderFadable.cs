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

    // URP/Lit properties
    static readonly int BaseColorId = Shader.PropertyToID("_BaseColor"); // URP Lit
    static readonly int ColorId = Shader.PropertyToID("_Color");     // fallback

    static readonly int SurfaceId = Shader.PropertyToID("_Surface");
    static readonly int SrcBlendId = Shader.PropertyToID("_SrcBlend");
    static readonly int DstBlendId = Shader.PropertyToID("_DstBlend");
    static readonly int ZWriteId = Shader.PropertyToID("_ZWrite");

    // cache để restore
    Material[][] _originalSharedMaterials;

    // material instance + màu gốc
    Material[][] _instancedMaterials;
    Color[][] _originalColors;

    // để destroy instance materials tránh leak
    readonly List<Material> _created = new();

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

        CreateMaterialInstancesAndMakeTransparent();
    }

    void OnDestroy()
    {
        RestoreOriginalMaterials();
        DestroyCreatedMaterials();
    }

    public void SetOccluding(bool occluding)
    {
        _target = occluding ? fadedAlpha : 1f;
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

            if (_target >= 0.999f) enabled = false;
        }
    }

    void CreateMaterialInstancesAndMakeTransparent()
    {
        int rCount = renderers.Length;

        _originalSharedMaterials = new Material[rCount][];
        _instancedMaterials = new Material[rCount][];
        _originalColors = new Color[rCount][];

        for (int i = 0; i < rCount; i++)
        {
            var r = renderers[i];
            if (!r) continue;

            var shared = r.sharedMaterials;
            _originalSharedMaterials[i] = shared;

            var inst = new Material[shared.Length];
            var cols = new Color[shared.Length];

            for (int m = 0; m < shared.Length; m++)
            {
                var sm = shared[m];
                if (!sm)
                {
                    inst[m] = null;
                    cols[m] = Color.white;
                    continue;
                }

                // tạo instance material
                var im = new Material(sm);
                _created.Add(im);

                // cache màu gốc (ưu tiên _BaseColor, fallback _Color)
                if (im.HasProperty(BaseColorId))
                    cols[m] = im.GetColor(BaseColorId);
                else if (im.HasProperty(ColorId))
                    cols[m] = im.GetColor(ColorId);
                else
                    cols[m] = Color.white;

                // convert transparent nếu có _Surface (URP/Lit-like)
                if (im.HasProperty(SurfaceId))
                {
                    im.SetFloat(SurfaceId, 1f); // Transparent
                    if (im.HasProperty(SrcBlendId)) im.SetInt(SrcBlendId, (int)BlendMode.SrcAlpha);
                    if (im.HasProperty(DstBlendId)) im.SetInt(DstBlendId, (int)BlendMode.OneMinusSrcAlpha);
                    if (im.HasProperty(ZWriteId)) im.SetInt(ZWriteId, 0);

                    im.renderQueue = (int)RenderQueue.Transparent;
                    im.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                    im.DisableKeyword("_ALPHATEST_ON");
                }

                inst[m] = im;
            }

            _instancedMaterials[i] = inst;
            _originalColors[i] = cols;

            // gán instance materials cho renderer
            r.materials = inst;
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

                if (mat.HasProperty(BaseColorId))
                    mat.SetColor(BaseColorId, c);
                else if (mat.HasProperty(ColorId))
                    mat.SetColor(ColorId, c);
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
            if (shared != null)
                r.sharedMaterials = shared;
        }
    }

    void DestroyCreatedMaterials()
    {
        for (int i = 0; i < _created.Count; i++)
        {
            if (_created[i])
                Destroy(_created[i]);
        }
        _created.Clear();
    }
}
