using Unity.Netcode;
using UnityEngine;

public class ServerFPSMonitor : MonoBehaviour
{
    private float timer;
    private int frames;

    private void Update()
    {
        if (NetworkManager.Singleton == null) return;
        if (!NetworkManager.Singleton.IsServer) return;

        frames++;
        timer += Time.unscaledDeltaTime;

        if (timer >= 1f)
        {
            int players = NetworkManager.Singleton.ConnectedClients.Count;
            long ram = System.GC.GetTotalMemory(false) / 1024 / 1024;

            Debug.Log($"SERVER FPS: {frames} | Players: {players} | RAM: {ram} MB");

            frames = 0;
            timer = 0f;
        }
    }
}