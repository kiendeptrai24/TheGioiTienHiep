using System;
using Unity.Netcode;
using UnityEngine;
using WorldMap.Baking;
using WorldMap.Data;
using WorldMap.Domain;
public class MapSpawn : TGTHMonoBehaviour
{
    public MapDataPreset mapDataPreset;
    public int walkable;
    public Transform player;
    public MapCell cell;
    int xPos;
    int zPos;
    public event Action<int, int> posPlayer;
    protected override void Awake()
    {
        base.Awake();
        PlayerNetManager.Instance.OnPlayerExiststed += OnPlayerExists;
    }

    private void OnPlayerExists(NetworkObject @object)
    {
        this.player = @object.transform;
    }
    protected override void Start()
    {
        base.Start();
    }
    [ContextMenu("Set Data")]
    public void Setdata()
    {
        mapDataPreset.Allocate();
        for (int i = 0; i < mapDataPreset.grid.width; i++)
        {
            for (int j = 0; j < mapDataPreset.grid.height; j++)
            {
                MapCell cell = new MapCell();
                cell.walkable = 1;
                cell.cost = 1;
                cell.position = new Vector3(i * mapDataPreset.grid.cellSize, 0, j * mapDataPreset.grid.cellSize);
                mapDataPreset.Set(i, j, cell);
            }
        }

    }
    public bool IsWalkableWorld(Vector3 worldPos)
    {
        if (mapDataPreset == null || mapDataPreset.grid == null) return false;

        float size = mapDataPreset.grid.cellSize;

        int x = Mathf.FloorToInt(worldPos.x / size);
        int z = Mathf.FloorToInt(worldPos.z / size);

        if (x < 0 || x >= mapDataPreset.grid.width || z < 0 || z >= mapDataPreset.grid.height)
            return false;

        return mapDataPreset.Get(x, z).walkable == 1;
    }

    private void Update()
    {
        if (player != null)
        {
            int x = (int)(player.position.x / mapDataPreset.grid.cellSize);
            int z = (int)(player.position.z / mapDataPreset.grid.cellSize);
            if (xPos == x && zPos == z) return;
            xPos = x;
            zPos = z;
            posPlayer?.Invoke(x, z);
            if (x >= 0 && x < mapDataPreset.grid.width && z >= 0 && z < mapDataPreset.grid.height)
            {
                cell = mapDataPreset.Get(x, z);
                walkable = cell.walkable;
            }
        }
    }
    public GridCoord WorldToGrid(Vector3 world) => GridMath.WorldToGrid(mapDataPreset.grid, world);
    public Vector3 GridToWorld(GridCoord c) => GridMath.GridToWorld(mapDataPreset.grid, c);
}