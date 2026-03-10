using System.Collections.Generic;
using UnityEngine;

public class HexIndicatorSpawner : TGTHMonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject indicatorPrefab;
    [SerializeField] private GameObject indicatorPrefabMiddle;
    [SerializeField] private Transform spawnParent;

    [Header("Grid Size")]
    [SerializeField] private int totalRows = 10;   // cao 10 hàng
    [SerializeField] private int rowA;         // hàng chẵn/lẻ đầu tiên có 5 ô
    [SerializeField] private int rowB;         // hàng tiếp theo có 4 ô

    [Header("Spacing")]
    [SerializeField] private float xSpacing = 1f;
    [SerializeField] private float zSpacing = 0.86f; // khoảng cách giữa các hàng hex
    private BattlePlaybackManager battlePlaybackManager;

    private readonly List<GameObject> spawnedObjects = new List<GameObject>();
    protected override void Awake()
    {
        base.Awake();
        battlePlaybackManager = BattlePlaybackManager.Instance;
        battlePlaybackManager.OnReadyGame += Show;
        battlePlaybackManager.OnStartGame += Hide;
    }
    private void Hide()
    {
        spawnParent.gameObject.SetActive(false);
    }
    private void Show()
    {
        spawnParent.gameObject.SetActive(true);
    }
    [ContextMenu("Spawn Hex Grid")]
    public void SpawnGrid()
    {
        ClearGrid();
        int middleIndex = totalRows / 2;
        for (int row = 0; row < totalRows; row++)
        {
            bool isFiveCellsRow = row % 2 == 0;
            int cellCount = isFiveCellsRow ? rowA : rowB;

            // Hàng 4 ô lệch vào giữa so với hàng 5 ô
            float offsetX = isFiveCellsRow == false ? 0f : xSpacing * 0.5f;
            if (row == middleIndex)
            {
                for (int col = 0; col < cellCount; col++)
                {
                    Vector3 localPos = new Vector3(
                        col * xSpacing + offsetX,
                        0f,
                        row * zSpacing
                    );
                    SpawnIndicator(localPos, indicatorPrefabMiddle, spawnParent);

                }
            }
            else
            {
                for (int col = 0; col < cellCount; col++)
                {
                    Vector3 localPos = new Vector3(
                        col * xSpacing + offsetX,
                        0f,
                        row * zSpacing
                    );
                    SpawnIndicator(localPos, indicatorPrefab, spawnParent);
                }
            }

        }
    }
    public void SpawnIndicator(Vector3 position, GameObject indicatorPrefab, Transform spawnParent)
    {
        GameObject obj = Instantiate(indicatorPrefab, spawnParent);
        obj.transform.localPosition = position;
        spawnedObjects.Add(obj);
    }

    [ContextMenu("Clear Hex Grid")]
    public void ClearGrid()
    {
        for (int i = spawnedObjects.Count - 1; i >= 0; i--)
        {
            if (spawnedObjects[i] != null)
            {
#if UNITY_EDITOR
                DestroyImmediate(spawnedObjects[i]);
#else
                Destroy(spawnedObjects[i]);
#endif
            }
        }

        spawnedObjects.Clear();
    }
}