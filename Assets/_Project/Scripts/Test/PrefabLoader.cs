using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PrefabLoader : IAssetLoader
{
    public PrefabLoader(string label)
    {
        this.label = label;
    }
    private Dictionary<string, GameObject> spriteLookup = new();
    private AsyncOperationHandle<GameObject[]> handle;

    public bool IsLoaded { get; private set; }

    private string label;

    public async Task Load()
    {
        if (IsLoaded) return;

        var handle = Addressables.LoadAssetsAsync<GameObject>(
            label,
            obj =>
            {

            }
        );
        var prefab = await handle.Task;

        if (handle.Status != AsyncOperationStatus.Succeeded || prefab == null)
        {
            Debug.LogError("Load sprite failed");
            return;
        }

        spriteLookup.Clear();

        foreach (var s in prefab)
        {
            spriteLookup[s.name] = s;
        }

        IsLoaded = true;
    }

    public GameObject Get(string name)
    {
        if (!IsLoaded)
        {
            Debug.LogWarning("PrefabLoader not loaded");
            return null;
        }
        if (string.IsNullOrEmpty(name))
            return null;

        if (spriteLookup.TryGetValue(name, out var s))
        {
            return s;
        }

        Debug.LogWarning($"Prefab not found: {name}");
        return null;
    }

    public void Unload()
    {
        if (!IsLoaded) return;

        spriteLookup.Clear();

        if (handle.IsValid())
            Addressables.Release(handle);

        IsLoaded = false;
    }
}