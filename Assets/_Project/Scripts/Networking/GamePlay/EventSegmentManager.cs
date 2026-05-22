using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventSegmentManager : TGTHNetworkBehaviour
{
    private ClientManager clientManager;
    private List<ISegmentSystem> segments = new();
    protected override void Awake()
    {
        base.Awake();
        segments = GetComponents<ISegmentSystem>().ToList();
    }
    protected override void Start()
    {
        base.Start();
        clientManager = ClientManager.Instance;
        clientManager.OnClientDataConnected += OnClientConnected;
        clientManager.OnClientDataDisconnected += OnClientDisconnected;
    }

    private void OnClientDisconnected(ClientData data)
    {
        if (!IsServer) return;

        foreach (var segment in segments)
            segment.DisconnectSegment(data);
    }

    private void OnClientConnected(ClientData data)
    {
        if (!IsServer) return;

        foreach (var segment in segments)
            segment.ConnectSegment(data);
    }
}