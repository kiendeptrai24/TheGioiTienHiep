using Unity.Netcode;
using UnityEngine;

public class GameNetworkStartup : MonoBehaviour
{
    [SerializeField] private Configuration config;
    private void Start()
    {
        if (config.IsServerBuild())
        {
            StartAsServer();
        }
        else    
        {
            StartAsBotClient();
        }
    }

    private void StartAsServer()
    {
        NetworkManager.Singleton.StartServer();
    }

    private void StartAsBotClient()
    {
        NetworkManager.Singleton.StartClient();
    }
}