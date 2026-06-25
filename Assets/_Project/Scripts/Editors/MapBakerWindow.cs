#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.IO;

public class MapBakerWindow : EditorWindow
{
    private Camera bakeCamera;

    private Vector2 worldMinXZ = new Vector2(0f, 0f);
    private Vector2 worldMaxXZ = new Vector2(1000f, 1000f);

    private float cameraHeight = 1000f;
    private float padding = 0f;

    private int maxResolution = 4096;
    private string fileName = "BakedWorldMap.png";

    private DefaultAsset outputFolder;

    private LayerMask cullingMask = ~0;
    private bool importAsSprite = true;

    [MenuItem("Tools/Map/Bake World Map PNG")]
    public static void Open()
    {
        GetWindow<MapBakerWindow>("Map Baker");
    }

    private void OnEnable()
    {
        if (outputFolder == null)
        {
            outputFolder = AssetDatabase.LoadAssetAtPath<DefaultAsset>("Assets");
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(8);

        bakeCamera = (Camera)EditorGUILayout.ObjectField(
            "Bake Camera",
            bakeCamera,
            typeof(Camera),
            true
        );

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("World Bounds theo X/Z", EditorStyles.boldLabel);

        worldMinXZ = EditorGUILayout.Vector2Field("World Min X/Z", worldMinXZ);
        worldMaxXZ = EditorGUILayout.Vector2Field("World Max X/Z", worldMaxXZ);

        if (GUILayout.Button("Lấy Bounds từ Object đang chọn"))
        {
            UseSelectedBounds();
        }

        EditorGUILayout.Space(8);

        cameraHeight = EditorGUILayout.FloatField("Camera Height", cameraHeight);
        padding = EditorGUILayout.FloatField("Padding", padding);

        maxResolution = EditorGUILayout.IntPopup(
            "Max Resolution",
            maxResolution,
            new[] { "1024", "2048", "4096", "8192" },
            new[] { 1024, 2048, 4096, 8192 }
        );

        EditorGUILayout.Space(8);

        cullingMask = LayerMaskField("Culling Mask", cullingMask);
        importAsSprite = EditorGUILayout.Toggle("Import As Sprite", importAsSprite);

        EditorGUILayout.Space(8);

        outputFolder = (DefaultAsset)EditorGUILayout.ObjectField(
            "Output Folder",
            outputFolder,
            typeof(DefaultAsset),
            false
        );

        fileName = EditorGUILayout.TextField("File Name", fileName);

        EditorGUILayout.Space(12);

        DrawPreviewInfo();

        EditorGUILayout.Space(8);

        if (GUILayout.Button("Bake PNG", GUILayout.Height(32)))
        {
            Bake();
        }
    }

    private void DrawPreviewInfo()
    {
        float width = GetWorldWidth();
        float depth = GetWorldDepth();

        if (width <= 0f || depth <= 0f)
        {
            EditorGUILayout.HelpBox("World Min / Max chưa hợp lệ.", MessageType.Warning);
            return;
        }

        Vector2Int outputSize = CalculateOutputSize(width, depth, maxResolution);

        EditorGUILayout.HelpBox(
            $"World Size: {width:0.##} x {depth:0.##} unit\n" +
            $"Output PNG: {outputSize.x} x {outputSize.y}px\n" +
            $"Aspect: {(width / depth):0.###}",
            MessageType.Info
        );
    }

    private float GetWorldWidth()
    {
        return (worldMaxXZ.x - worldMinXZ.x) + padding * 2f;
    }

    private float GetWorldDepth()
    {
        return (worldMaxXZ.y - worldMinXZ.y) + padding * 2f;
    }

    private Vector2Int CalculateOutputSize(float worldWidth, float worldDepth, int maxSide)
    {
        int width;
        int height;

        if (worldWidth >= worldDepth)
        {
            width = maxSide;
            height = Mathf.RoundToInt(maxSide * (worldDepth / worldWidth));
        }
        else
        {
            height = maxSide;
            width = Mathf.RoundToInt(maxSide * (worldWidth / worldDepth));
        }

        width = Mathf.Max(1, width);
        height = Mathf.Max(1, height);

        return new Vector2Int(width, height);
    }

    private void Bake()
    {
        if (bakeCamera == null)
        {
            Debug.LogError("Chưa gán Bake Camera.");
            return;
        }

        float worldWidth = GetWorldWidth();
        float worldDepth = GetWorldDepth();

        if (worldWidth <= 0f || worldDepth <= 0f)
        {
            Debug.LogError("World Min / Max không hợp lệ.");
            return;
        }

        Vector2Int outputSize = CalculateOutputSize(worldWidth, worldDepth, maxResolution);

        string folderPath = GetOutputFolderPath();
        string safeFileName = GetSafePngFileName(fileName);

        string assetPath = Path.Combine(folderPath, safeFileName).Replace("\\", "/");
        assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);

        string fullPath = Path.GetFullPath(assetPath);

        Vector3 oldPosition = bakeCamera.transform.position;
        Quaternion oldRotation = bakeCamera.transform.rotation;

        bool oldOrthographic = bakeCamera.orthographic;
        float oldOrthographicSize = bakeCamera.orthographicSize;
        float oldAspect = bakeCamera.aspect;

        CameraClearFlags oldClearFlags = bakeCamera.clearFlags;
        Color oldBackgroundColor = bakeCamera.backgroundColor;

        float oldNearClipPlane = bakeCamera.nearClipPlane;
        float oldFarClipPlane = bakeCamera.farClipPlane;

        int oldCullingMask = bakeCamera.cullingMask;

        RenderTexture oldTargetTexture = bakeCamera.targetTexture;
        RenderTexture oldActive = RenderTexture.active;

        RenderTexture rt = null;
        Texture2D tex = null;

        try
        {
            Vector3 center = new Vector3(
                (worldMinXZ.x + worldMaxXZ.x) * 0.5f,
                cameraHeight,
                (worldMinXZ.y + worldMaxXZ.y) * 0.5f
            );

            bakeCamera.transform.position = center;
            bakeCamera.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

            bakeCamera.orthographic = true;
            bakeCamera.orthographicSize = worldDepth * 0.5f;
            bakeCamera.aspect = worldWidth / worldDepth;

            // Giữ background gốc của camera:
            // - Không đổi clearFlags
            // - Không đổi backgroundColor
            //
            // Nhưng vẫn chỉnh clip plane để tránh ground/map bị cắt.
            bakeCamera.nearClipPlane = 0.1f;
            bakeCamera.farClipPlane = cameraHeight + 5000f;

            bakeCamera.cullingMask = cullingMask.value;

            rt = new RenderTexture(outputSize.x, outputSize.y, 24, RenderTextureFormat.ARGB32);
            rt.name = "WorldMap_Bake_RT";
            rt.antiAliasing = 1;
            rt.Create();

            bakeCamera.targetTexture = rt;
            RenderTexture.active = rt;

            bakeCamera.Render();

            tex = new Texture2D(outputSize.x, outputSize.y, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, outputSize.x, outputSize.y), 0, 0);
            tex.Apply();

            byte[] png = tex.EncodeToPNG();
            File.WriteAllBytes(fullPath, png);

            AssetDatabase.ImportAsset(assetPath);
            ApplyImportSettings(assetPath);

            Debug.Log($"Đã bake map ra: {assetPath}");
        }
        finally
        {
            bakeCamera.transform.position = oldPosition;
            bakeCamera.transform.rotation = oldRotation;

            bakeCamera.orthographic = oldOrthographic;
            bakeCamera.orthographicSize = oldOrthographicSize;
            bakeCamera.aspect = oldAspect;

            bakeCamera.clearFlags = oldClearFlags;
            bakeCamera.backgroundColor = oldBackgroundColor;

            bakeCamera.nearClipPlane = oldNearClipPlane;
            bakeCamera.farClipPlane = oldFarClipPlane;

            bakeCamera.cullingMask = oldCullingMask;

            bakeCamera.targetTexture = oldTargetTexture;
            RenderTexture.active = oldActive;

            if (rt != null)
            {
                rt.Release();
                DestroyImmediate(rt);
            }

            if (tex != null)
            {
                DestroyImmediate(tex);
            }

            AssetDatabase.Refresh();
        }
    }

    private void UseSelectedBounds()
    {
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects == null || selectedObjects.Length == 0)
        {
            Debug.LogWarning("Chưa chọn object nào trong Hierarchy.");
            return;
        }

        bool hasBounds = false;
        Bounds bounds = new Bounds();

        foreach (GameObject go in selectedObjects)
        {
            Renderer[] renderers = go.GetComponentsInChildren<Renderer>(true);

            foreach (Renderer renderer in renderers)
            {
                if (!renderer.enabled)
                    continue;

                if (!hasBounds)
                {
                    bounds = renderer.bounds;
                    hasBounds = true;
                }
                else
                {
                    bounds.Encapsulate(renderer.bounds);
                }
            }

            Terrain[] terrains = go.GetComponentsInChildren<Terrain>(true);

            foreach (Terrain terrain in terrains)
            {
                if (terrain.terrainData == null)
                    continue;

                Vector3 terrainSize = terrain.terrainData.size;
                Vector3 terrainCenter = terrain.transform.position + terrainSize * 0.5f;

                Bounds terrainBounds = new Bounds(terrainCenter, terrainSize);

                if (!hasBounds)
                {
                    bounds = terrainBounds;
                    hasBounds = true;
                }
                else
                {
                    bounds.Encapsulate(terrainBounds);
                }
            }
        }

        if (!hasBounds)
        {
            Debug.LogWarning("Object đang chọn không có Renderer hoặc Terrain nào.");
            return;
        }

        worldMinXZ = new Vector2(bounds.min.x, bounds.min.z);
        worldMaxXZ = new Vector2(bounds.max.x, bounds.max.z);

        Debug.Log($"Đã lấy bounds: Min {worldMinXZ}, Max {worldMaxXZ}");
    }

    private string GetOutputFolderPath()
    {
        if (outputFolder == null)
            return "Assets";

        string path = AssetDatabase.GetAssetPath(outputFolder);

        if (string.IsNullOrEmpty(path) || !AssetDatabase.IsValidFolder(path))
            return "Assets";

        return path;
    }

    private string GetSafePngFileName(string rawFileName)
    {
        if (string.IsNullOrWhiteSpace(rawFileName))
            rawFileName = "BakedWorldMap.png";

        rawFileName = rawFileName.Trim();

        foreach (char c in Path.GetInvalidFileNameChars())
        {
            rawFileName = rawFileName.Replace(c.ToString(), "_");
        }

        if (!rawFileName.ToLower().EndsWith(".png"))
            rawFileName += ".png";

        return rawFileName;
    }

    private void ApplyImportSettings(string assetPath)
    {
        TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;

        if (importer == null)
            return;

        importer.mipmapEnabled = false;
        importer.isReadable = false;
        importer.alphaIsTransparency = false;
        importer.maxTextureSize = maxResolution;

        if (importAsSprite)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
        }
        else
        {
            importer.textureType = TextureImporterType.Default;
        }

        importer.SaveAndReimport();
    }

    private static LayerMask LayerMaskField(string label, LayerMask selected)
    {
        string[] layerNames = InternalEditorUtility.layers;

        int editorMask = 0;

        for (int i = 0; i < layerNames.Length; i++)
        {
            int layer = LayerMask.NameToLayer(layerNames[i]);

            if ((selected.value & (1 << layer)) != 0)
            {
                editorMask |= 1 << i;
            }
        }

        editorMask = EditorGUILayout.MaskField(label, editorMask, layerNames);

        int realMask = 0;

        for (int i = 0; i < layerNames.Length; i++)
        {
            if ((editorMask & (1 << i)) != 0)
            {
                int layer = LayerMask.NameToLayer(layerNames[i]);
                realMask |= 1 << layer;
            }
        }

        selected.value = realMask;
        return selected;
    }
}
#endif