using UnityEngine;

[DisallowMultipleComponent]
public class PlayerVisibilityTintURP : TGTHNetworkBehaviour
{
    [Header("Renderers")]
    public Renderer[] renderers;

    [Header("Materials")]
    public Material occludedMaterial; // ✅ kéo material sáng/xanh cho player bị che

    [Header("Link")]
    public CameraOcclusionFader fader;

    Material[][] _originalShared;

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

        CacheOriginal();

    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner) return;
        if (!fader) fader = FindAnyObjectByType<CameraOcclusionFader>();
        if (fader) fader.OnOccluded += OnOccludedChanged;

    }
    protected void OnDestroy()
    {
        if (!IsOwner) return;
        if (fader) fader.OnOccluded -= OnOccludedChanged;
        RestoreOriginal();
    }

    void CacheOriginal()
    {
        _originalShared = new Material[renderers.Length][];

        for (int i = 0; i < renderers.Length; i++)
        {
            if (!renderers[i]) continue;
            _originalShared[i] = renderers[i].sharedMaterials;
        }
    }

    void OnOccludedChanged(bool occluded)
    {
        if (occluded) ApplyOccludedMaterial();
        else RestoreOriginal();
    }

    void ApplyOccludedMaterial()
    {
        if (!occludedMaterial || renderers == null) return;

        for (int i = 0; i < renderers.Length; i++)
        {
            var r = renderers[i];
            if (!r) continue;

            var slots = r.sharedMaterials;
            for (int m = 0; m < slots.Length; m++)
                slots[m] = occludedMaterial;

            r.sharedMaterials = slots;
        }
    }

    void RestoreOriginal()
    {
        if (_originalShared == null || renderers == null) return;

        for (int i = 0; i < renderers.Length; i++)
        {
            var r = renderers[i];
            if (!r) continue;

            var shared = _originalShared[i];
            if (shared != null)
                r.sharedMaterials = shared;
        }
    }
}
