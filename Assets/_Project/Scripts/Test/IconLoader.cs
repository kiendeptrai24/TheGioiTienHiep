using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlayFab.Internal;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class IconLoader : IAssetLoader
{
    public IconLoader(string address)
    {
        this.address = address;
    }
    private Dictionary<string, Sprite> spriteLookup = new();
    private AsyncOperationHandle<Sprite[]> handle;

    public bool IsLoaded { get; private set; }

    private string address;

    public async Task Load()
    {
        if (IsLoaded) return;

        handle = Addressables.LoadAssetAsync<Sprite[]>(address);
        var sprites = await handle.Task;

        if (handle.Status != AsyncOperationStatus.Succeeded || sprites == null)
        {
            Debug.LogError("Load sprite failed");
            return;
        }
        handle.Completed += OnComplete;
        spriteLookup.Clear();

        foreach (var s in sprites)
        {
            spriteLookup[s.name] = s;
        }

        IsLoaded = true;
    }

    private void OnComplete(AsyncOperationHandle<Sprite[]> handle)
    {
        
    }

    public Sprite Get(string name)
    {
        if (!IsLoaded)
        {
            Debug.LogWarning("IconLoader not loaded");
            return null;
        }
        if (string.IsNullOrEmpty(name))
            return null;
        if (spriteLookup.TryGetValue(name, out var s))
        {
            return s;
        }

        Debug.LogWarning($"Sprite not found: {name}");
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