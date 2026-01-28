using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class MinimapManger : TGTHMonoBehaviour
{
    public Camera minimapCamera;
    public CinemachineCamera cinemachineCamera;
    public Transform minimapTarget;
    [SerializeField] private Transform player;
    [SerializeField] private List<MinimapController> controllers;
    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
    }
    protected override void Start()
    {
        base.Start();
    }
    public void SetPlayer(Transform player)
    {
        this.player = player;
        if (controllers != null)
            foreach (var controller in controllers)
            {
                controller.SetFollowPlayer(player);
            }
    }

    protected override void LoadComponent()
    {
        base.LoadComponent();
        if (minimapCamera == null) minimapCamera = GetComponentInChildren<Camera>();
        if (cinemachineCamera == null) cinemachineCamera = GetComponentInChildren<CinemachineCamera>();
    }
}
