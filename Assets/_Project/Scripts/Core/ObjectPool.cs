using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : TGTHMonoBehaviour
{
    public static ObjectPool Instance { get; private set; }
    [SerializeField] private int poolSize = 10;
    private Dictionary<GameObject, Queue<GameObject>> poolDictionary = new();


    protected override void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    protected override void Start()
    {

    }
    public GameObject GetObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (poolDictionary.ContainsKey(prefab) == false)
            InitializeNewPool(prefab);

        if (poolDictionary[prefab].Count == 0)
            CreateNewObject(prefab);

        GameObject objectToGet = poolDictionary[prefab].Dequeue();
        objectToGet.transform.position = position;
        objectToGet.transform.rotation = rotation;
        objectToGet.transform.parent = null;
        objectToGet.SetActive(true);

        return objectToGet;
    }
    public void ReturnObject(GameObject objectToReturn, float delay = .001f) => StartCoroutine(DelayReturn(objectToReturn, delay));
    private IEnumerator DelayReturn(GameObject objectToReturn, float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnToPool(objectToReturn);
    }
    private void ReturnToPool(GameObject objectToReturn)
    {
        GameObject originalToReturn = objectToReturn.GetComponent<PooledObject>().originalPrefab;
        objectToReturn.SetActive(false);
        objectToReturn.transform.parent = transform;
        poolDictionary[originalToReturn].Enqueue(objectToReturn);
    }
    private void InitializeNewPool(GameObject prefab)
    {
        poolDictionary[prefab] = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            CreateNewObject(prefab);
        }
    }

    private void CreateNewObject(GameObject prefab)
    {
        GameObject newObject = Instantiate(prefab, transform);
        newObject.AddComponent<PooledObject>().originalPrefab = prefab;
        newObject.SetActive(false);
        poolDictionary[prefab].Enqueue(newObject);
    }
}