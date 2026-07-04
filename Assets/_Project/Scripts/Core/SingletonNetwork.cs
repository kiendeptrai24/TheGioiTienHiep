using System;
using Unity.Netcode;
using UnityEngine;

public class SingletonNetwork<T> : TGTHNetworkBehaviour where T : TGTHNetworkBehaviour
{
    private static T _instance;
    private static object _lock = new object();
    private static bool _isQuitting = false;
    public static T Instance
    {
        get
        {
            if (_isQuitting) return null;

            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<T>();

                    if (_instance == null)
                    {
                        GameObject singletonObj = new GameObject(typeof(T).Name);
                        // Đảm bảo GameObject có NetworkObject để IsServer hoạt động trên Dedicated Server
                        if (singletonObj.GetComponent<NetworkObject>() == null)
                        {
                            singletonObj.AddComponent<NetworkObject>();
                        }
                        _instance = singletonObj.AddComponent<T>();
                        Debug.LogWarning($"[SingletonNetwork] Created new GameObject for '{typeof(T).Name}' (no scene instance found). IsServer={_instance.IsServer}");
                    }
                }
                return _instance;
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _instance = this as T;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (_instance == this as T)
            _instance = null;
    }

    protected virtual void OnApplicationQuit()
    {
        _isQuitting = true;
    }

    protected virtual void OnDestroy()
    {
        _isQuitting = true;
        if (_instance == this as T)
            _instance = null;
    }
}