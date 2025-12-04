using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    private NetworkManager networkManager;
    [SerializeField] private Button startClientBtn;
    [SerializeField] private Button startHostBtn;
    [SerializeField] private Button startServerBtn;
    [SerializeField] private Button disconnectBtn;

    private void Awake()
    {
        startClientBtn.onClick.AddListener(OnStartClientClicked);
        startHostBtn.onClick.AddListener(OnStartHostClicked);
        startServerBtn.onClick.AddListener(OnStartServerClicked);
        disconnectBtn.onClick.AddListener(OnDisconnectClicked);
        ShowLoginPanel(true);
    }
    private void Start() {
        networkManager = NetworkManager.Singleton;
    }
    private void OnDisconnectClicked()
    {
        networkManager.Shutdown();
        ShowLoginPanel(true);
    }

    private void OnStartServerClicked()
    {
        if(networkManager.StartServer())
        {
            ShowLoginPanel(false);
        }
    }

    private void OnStartHostClicked()
    {
        
        if(networkManager.StartHost())
        {
            ShowLoginPanel(false);
        }
    }

    private void OnStartClientClicked()
    {
        if(networkManager.StartClient())
        {
            ShowLoginPanel(false);
        }
    }

    private void ShowDisconnectButton(bool show)
    {
        disconnectBtn.gameObject.SetActive(show);
    }

    private void ShowLoginPanel(bool show)
    {
        startClientBtn.gameObject.SetActive(show);
        startHostBtn.gameObject.SetActive(show);
        startServerBtn.gameObject.SetActive(show);
        ShowDisconnectButton(!show);
    }
}
