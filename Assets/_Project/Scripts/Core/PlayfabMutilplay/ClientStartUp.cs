using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class ClientStartUp : Singleton<ClientStartUp>
{
    [SerializeField] private Configuration configuration;

    protected override void Awake()
    {
        base.Awake();
        // if (configuration.IsClientRemoteBuild())
        // {
        //     ConnectRemoteClient();
        // }
    }

    private void ConnectRemoteClient()
    {
        string ip = configuration.ipAddress;
        ushort port = configuration.port;

        var utp = NetworkManager.Singleton.NetworkConfig.NetworkTransport as UnityTransport;
        if (utp == null)
        {
            Debug.LogError("[ClientStartUp] UnityTransport not found on NetworkManager.");
            return;
        }

        if (string.IsNullOrEmpty(ip))
        {
            Debug.LogError("[ClientStartUp] IP address is empty. Check Configuration.");
            return;
        }

        if (port == 0)
        {
            Debug.LogError("[ClientStartUp] Port is 0. Check Configuration.");
            return;
        }

        Debug.Log($"[ClientStartUp] Connecting to {ip}:{port}");
        utp.SetConnectionData(ip, port);
    }
}
