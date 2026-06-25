using UnityEngine;

[DisallowMultipleComponent]
public class PlayerVisibilityTintURP : TGTHNetworkBehaviour
{
#if !UNITY_SERVER
    static readonly Material[] EmptyMaterials = System.Array.Empty<Material>();

    [Header("Renderers")]
    public Renderer[] renderers;

    [Header("Materials")]
    public Material occludedMaterial; // ✅ kéo material sáng/xanh cho player bị che

    [Header("Link")]
    public CameraOcclusionFader fader;

    Material[][] _originalShared;
    Material[][] _occludedShared;
    bool _isOccludedApplied;

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

        CacheMaterials();
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner) return;
        if (!fader) fader = CameraOcclusionFader.Instance;
        if (fader) fader.OnOccluded += OnOccludedChanged;

    }
    protected void OnDestroy()
    {
        if (!IsOwner) return;
        if (fader) fader.OnOccluded -= OnOccludedChanged;
        RestoreOriginal();
    }

    void CacheMaterials()
    {
        _originalShared = new Material[renderers.Length][];
        _occludedShared = new Material[renderers.Length][];

        for (int i = 0; i < renderers.Length; i++)
        {
            var renderer = renderers[i];
            if (!renderer)
            {
                _originalShared[i] = EmptyMaterials;
                _occludedShared[i] = EmptyMaterials;
                continue;
            }

            var shared = renderer.sharedMaterials;
            _originalShared[i] = shared;

            if (shared == null || shared.Length == 0)
            {
                _occludedShared[i] = EmptyMaterials;
                continue;
            }

            var occluded = new Material[shared.Length];
            for (int m = 0; m < occluded.Length; m++)
                occluded[m] = occludedMaterial;

            _occludedShared[i] = occluded;
        }
    }

    void OnOccludedChanged(bool occluded)
    {
        if (occluded) ApplyOccludedMaterial();
        else RestoreOriginal();
    }

    void ApplyOccludedMaterial()
    {
        if (_isOccludedApplied || !occludedMaterial || renderers == null || _occludedShared == null) return;
        _isOccludedApplied = true;

        for (int i = 0; i < renderers.Length; i++)
        {
            var r = renderers[i];
            if (!r) continue;
            r.sharedMaterials = _occludedShared[i];
        }
    }

    void RestoreOriginal()
    {
        if (!_isOccludedApplied || _originalShared == null || renderers == null) return;
        _isOccludedApplied = false;

        for (int i = 0; i < renderers.Length; i++)
        {
            var r = renderers[i];
            if (!r) continue;

            var shared = _originalShared[i];
            if (shared != null)
                r.sharedMaterials = shared;
        }
    }
#endif
}
