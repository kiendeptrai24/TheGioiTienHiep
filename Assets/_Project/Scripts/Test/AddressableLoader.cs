using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
public enum AddressableLoaderType
{
    Sprite,
    Prefab,
}
public class AddressableLoader : Singleton<AddressableLoader>
{
    private Dictionary<string, IAssetLoader> loaders = new();

    protected override async void Awake()
    {
        base.Awake();
        var iconLoader = new IconLoader("Icons/#2 - Transparent Icons & Drop Shadow");
        var prefabLoader = new PrefabLoader("Prefab");
        Register(AddressableLoaderType.Sprite.ToString(), iconLoader);
        Register(AddressableLoaderType.Prefab.ToString(), prefabLoader);
        await LoadAll();
    }
    public void Register(string key, IAssetLoader loader)
    {
        if (loaders.ContainsKey(key))
        {
            Debug.LogWarning($"Loader {key} already registered");
            return;
        }

        loaders.Add(key, loader);
    }

    public T GetLoader<T>(string key) where T : class, IAssetLoader
    {
        if (loaders.TryGetValue(key, out var loader))
            return loader as T;

        Debug.LogError($"Loader not found: {key}");
        return null;
    }

    public async Task Load(string key)
    {
        if (loaders.TryGetValue(key, out var loader))
        {
            if (!loader.IsLoaded)
                await loader.Load();
        }
    }

    public void Unload(string key)
    {
        if (loaders.TryGetValue(key, out var loader))
        {
            loader.Unload();
        }
    }

    public async Task LoadAll()
    {
        foreach (var loader in loaders.Values)
        {
            if (!loader.IsLoaded)
                await loader.Load();
        }
    }

    public void UnloadAll()
    {
        foreach (var loader in loaders.Values)
        {
            loader.Unload();
        }
    }
}