using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[DisallowMultipleComponent]
public class OccluderFadable : TGTHMonoBehaviour
{
#if !UNITY_SERVER
    [Header("Fade")]
    [Range(0f, 1f)] public float fadedAlpha = 0.25f;
    public float fadeSpeed = 8f;

    [Header("Renderers")]
    public Renderer[] renderers;

    const byte ColorPropertyNone = 0;
    const byte ColorPropertyBase = 1;
    const byte ColorPropertyMain = 2;

    static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    static readonly int ColorId = Shader.PropertyToID("_Color");
    static readonly int SurfaceId = Shader.PropertyToID("_Surface");
    static readonly int SrcBlendId = Shader.PropertyToID("_SrcBlend");
    static readonly int DstBlendId = Shader.PropertyToID("_DstBlend");
    static readonly int ZWriteId = Shader.PropertyToID("_ZWrite");

    float _current = 1f;
    float _target = 1f;
    float _lastAppliedAlpha = -1f;

    Material[][] _originalSharedMaterials;
    Material[][] _transparentMaterials;
    Color[][] _originalColors;
    byte[][] _colorProperties;
    MaterialPropertyBlock[][] _propertyBlocks;

    readonly List<Material> _created = new();

    bool _occluding;
    bool _initialized;
    bool _usingTransparentMaterials;

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

        enabled = false;
    }

    void OnDestroy()
    {
        RestoreOriginalMaterials();
        DestroyCreatedMaterials();
    }

    public void SetOccluding(bool occluding)
    {
        if (_occluding == occluding) return;
        Debug.Log($"OccluderFadable.SetOccluding({occluding}) called on {gameObject.name}");
        _occluding = occluding;
        _target = _occluding ? fadedAlpha : 1f;

        if (_occluding)
        {
            EnsureInitialized();
            AssignTransparentMaterials();
        }

        enabled = _initialized;
    }

    void Update()
    {
        if (!_initialized)
        {
            enabled = false;
            return;
        }

        float next = Mathf.MoveTowards(_current, _target, fadeSpeed * Time.deltaTime);
        if (!Mathf.Approximately(next, _current))
        {
            _current = next;
            ApplyAlpha(_current);
        }

        if (Mathf.Abs(_current - _target) >= 0.001f) return;

        _current = _target;
        ApplyAlpha(_current);

        if (_target < 0.999f)
        {
            enabled = false;
            return;
        }

        RestoreOriginalMaterials();
        enabled = false;
    }

    void EnsureInitialized()
    {
        if (_initialized) return;

        CreateTransparentMaterials();
        _initialized = true;
    }

    void CreateTransparentMaterials()
    {
        int rendererCount = renderers.Length;

        _originalSharedMaterials = new Material[rendererCount][];
        _transparentMaterials = new Material[rendererCount][];
        _originalColors = new Color[rendererCount][];
        _colorProperties = new byte[rendererCount][];
        _propertyBlocks = new MaterialPropertyBlock[rendererCount][];

        for (int i = 0; i < rendererCount; i++)
        {
            var renderer = renderers[i];
            if (!renderer) continue;

            var sharedMaterials = renderer.sharedMaterials;
            _originalSharedMaterials[i] = sharedMaterials;

            int materialCount = sharedMaterials.Length;
            var transparentMaterials = new Material[materialCount];
            var colors = new Color[materialCount];
            var colorProperties = new byte[materialCount];
            var propertyBlocks = new MaterialPropertyBlock[materialCount];

            for (int m = 0; m < materialCount; m++)
            {
                var sharedMaterial = sharedMaterials[m];
                if (!sharedMaterial)
                {
                    colors[m] = Color.white;
                    continue;
                }

                var transparentMaterial = new Material(sharedMaterial);
                ConfigureTransparentMaterial(transparentMaterial);

                _created.Add(transparentMaterial);
                transparentMaterials[m] = transparentMaterial;
                propertyBlocks[m] = new MaterialPropertyBlock();

                if (transparentMaterial.HasProperty(BaseColorId))
                {
                    colorProperties[m] = ColorPropertyBase;
                    colors[m] = transparentMaterial.GetColor(BaseColorId);
                }
                else if (transparentMaterial.HasProperty(ColorId))
                {
                    colorProperties[m] = ColorPropertyMain;
                    colors[m] = transparentMaterial.GetColor(ColorId);
                }
                else
                {
                    colorProperties[m] = ColorPropertyNone;
                    colors[m] = Color.white;
                }
            }

            _transparentMaterials[i] = transparentMaterials;
            _originalColors[i] = colors;
            _colorProperties[i] = colorProperties;
            _propertyBlocks[i] = propertyBlocks;
        }
    }

    static void ConfigureTransparentMaterial(Material material)
    {
        if (!material) return;

        if (material.HasProperty(SurfaceId)) material.SetFloat(SurfaceId, 1f);
        if (material.HasProperty(SrcBlendId)) material.SetInt(SrcBlendId, (int)BlendMode.SrcAlpha);
        if (material.HasProperty(DstBlendId)) material.SetInt(DstBlendId, (int)BlendMode.OneMinusSrcAlpha);
        if (material.HasProperty(ZWriteId)) material.SetInt(ZWriteId, 0);

        material.renderQueue = (int)RenderQueue.Transparent;
        material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
    }

    void AssignTransparentMaterials()
    {
        if (!_initialized || _usingTransparentMaterials) return;

        for (int i = 0; i < renderers.Length; i++)
        {
            var renderer = renderers[i];
            var materials = _transparentMaterials[i];
            if (!renderer || materials == null) continue;

            renderer.sharedMaterials = materials;
        }

        _usingTransparentMaterials = true;
        _lastAppliedAlpha = -1f;
        ApplyAlpha(_current);
    }

    void ApplyAlpha(float alpha)
    {
        if (!_initialized || !_usingTransparentMaterials) return;
        if (Mathf.Abs(alpha - _lastAppliedAlpha) < 0.001f) return;

        for (int i = 0; i < renderers.Length; i++)
        {
            var renderer = renderers[i];
            var colors = _originalColors[i];
            var colorProperties = _colorProperties[i];
            var propertyBlocks = _propertyBlocks[i];
            if (!renderer || colors == null || colorProperties == null || propertyBlocks == null) continue;

            int materialCount = colorProperties.Length;
            for (int m = 0; m < materialCount; m++)
            {
                byte colorProperty = colorProperties[m];
                var block = propertyBlocks[m];
                if (colorProperty == ColorPropertyNone || block == null) continue;

                block.Clear();

                var color = colors[m];
                color.a = alpha;

                if (colorProperty == ColorPropertyBase) block.SetColor(BaseColorId, color);
                else block.SetColor(ColorId, color);

                renderer.SetPropertyBlock(block, m);
            }
        }

        _lastAppliedAlpha = alpha;
    }

    void RestoreOriginalMaterials()
    {
        if (_originalSharedMaterials == null || renderers == null) return;

        for (int i = 0; i < renderers.Length; i++)
        {
            var renderer = renderers[i];
            if (!renderer) continue;

            var sharedMaterials = _originalSharedMaterials[i];
            if (sharedMaterials != null) renderer.sharedMaterials = sharedMaterials;

            var propertyBlocks = _propertyBlocks != null ? _propertyBlocks[i] : null;
            if (propertyBlocks == null) continue;

            for (int m = 0; m < propertyBlocks.Length; m++)
                renderer.SetPropertyBlock(null, m);
        }

        _usingTransparentMaterials = false;
        _lastAppliedAlpha = -1f;
    }

    void DestroyCreatedMaterials()
    {
        foreach (var material in _created)
            if (material) Destroy(material);

        _created.Clear();
    }
#endif
}
