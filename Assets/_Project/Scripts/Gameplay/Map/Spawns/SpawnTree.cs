using UnityEngine;

public class SpawnTree : MonoBehaviour
{
    [SerializeField] private SpawnSettings settings;
    [SerializeField] private GameObject prefab;
    [SerializeField] private SpawnManager spawnManager;
    private void Awake()
    {
        spawnManager = GetComponent<SpawnManager>();
    }
    private void Start()
    {
        ISpawnArea area = new RectSpawnArea(new Vector3(700, 0, 700), new Vector2(200, 200));
        ISpawnPattern pattern = new RandomSpawnPattern();

        // spawnManager.Spawn(prefab, area, pattern, settings);
    }

}