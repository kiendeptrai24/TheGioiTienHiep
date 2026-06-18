using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class ClientStartUp : Singleton<ClientStartUp>
{
    [SerializeField] private Configuration configuration;

    protected override void Awake()
    {
        if (configuration.IsClientRemoteBuild())
        {
            ConnectRemoteClient();
        }
    }

    private void ConnectRemoteClient()
    {
        string ip;
        ushort port;

        ip = configuration.ipAddress;
        port = configuration.port;
        var utp = NetworkManager.Singleton.NetworkConfig.NetworkTransport as UnityTransport;
        if (utp == null)
        {
            return;
        }

        utp.SetConnectionData(ip, port);
    }
}
