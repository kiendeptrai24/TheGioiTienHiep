using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class CameraSwitchManager : Singleton<CameraSwitchManager>
{
    public string cameraPlayerName;
    public Vector3 cameraPlayerRotation;
    [SerializeField] private CinemachineCamera cinemachine;
    private CinemachineFollow follow;
    public List<CameraPoint> switchSetups;
    public Dictionary<string, CameraPoint> cameraPoints = new();
    override protected void Awake()
    {
        base.Awake();
        LoadComponent();
        foreach (var point in switchSetups)
        {
            cameraPoints.Add(point.target.name, point);
        }
        PlayerNetManager.Instance.OnPlayerExiststed += OnPlayerExiststed;
    }

    private void OnPlayerExiststed(NetworkObject playerNet)
    {
        var playerPoint = new CameraPoint();
        playerPoint.target = playerNet.transform;
        playerPoint.rotation = cameraPlayerRotation;

        cameraPoints.Add(cameraPlayerName, playerPoint);
    }

    override protected void Start()
    {
        base.Start();
    }
    public void SwitchCameraPosition(string targetName)
    {
        if (cameraPoints.TryGetValue(targetName, out var targetPoint))
        {
            cinemachine.Follow = targetPoint.target;
            cinemachine.LookAt = targetPoint.target;
            //follow.FollowOffset = targetPoint.rotation;
            return;
        }
    }
    [ContextMenu("switch to battle")]
    public void SwitchToBattle() => SwitchCameraPosition("battlepoint");
    [ContextMenu("switch to base")]
    public void SwitchToBase() => SwitchCameraPosition("basepoint");
    [ContextMenu("switch to player")]
    public void SwitchToPlayer() => SwitchCameraPosition("player");

    public void ResetToPlayer()
    {
        SwitchCameraPosition("player");
    }
    override protected void LoadComponent()
    {
        base.LoadComponent();
        follow = GetComponent<CinemachineFollow>();
    }
}
