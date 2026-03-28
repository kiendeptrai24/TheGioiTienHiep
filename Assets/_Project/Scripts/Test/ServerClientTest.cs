using Unity.Netcode;
using UnityEngine;
public enum ServerClientType
{
    Server,
    Client
}
public class ServerClientTest : TGTHMonoBehaviour
{
    public ServerClientType type;
    private NetworkManager networkManager;
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
        networkManager = NetworkManager.Singleton;
        if (type == ServerClientType.Server)
        {
            networkManager.StartServer();
        }
    }
}
