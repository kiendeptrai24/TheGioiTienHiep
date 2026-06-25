using FeatureToggles;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class PlayfabClientRuntimeService
{
    public bool TryStartNetworkSession()
    {
        if (Configuration.Instance.startwithHost)
        {
            return StartHostSession();
        }

        ConfigureClientTransportIfNeeded();
        return StartClientSession();
    }

    public void ShutdownNetworkIfNeeded()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
        }
    }

    public void ResetClientSystems()
    {
        ScreenManagerHub.Instance.ResetAll();
        FeatureManager.Instance.Reset();
    }

    public void ClearBattleHistory()
    {
        if (BattleHistoryController.Instance != null)
        {
            BattleHistoryController.Instance.ClearBattleHistory();
        }
    }

    private bool StartHostSession()
    {
        bool started = NetworkManager.Singleton.StartHost();
        if (!started)
        {
            Debug.LogError("StartHost failed, keep LoadingScene open.");
        }

        return started;
    }

    private void ConfigureClientTransportIfNeeded()
    {
        if (!Configuration.Instance.IsClientRemoteBuild())
        {
            return;
        }

        var utp = NetworkManager.Singleton.NetworkConfig.NetworkTransport as UnityTransport;
        if (utp == null)
        {
            return;
        }

        var config = Configuration.Instance;
        if (config == null)
        {
            return;
        }

        if (string.IsNullOrEmpty(config.ipAddress) || config.port == 0)
        {
            Debug.Log("Invalid IP address or port in configuration. Please check the settings.");
            return;
        }

        utp.SetConnectionData(config.ipAddress, config.port);
    }

    private bool StartClientSession()
    {
        bool started = NetworkManager.Singleton.StartClient();
        if (!started)
        {
            Debug.LogError("StartClient failed, keep LoadingScene open.");
        }

        return started;
    }
}
