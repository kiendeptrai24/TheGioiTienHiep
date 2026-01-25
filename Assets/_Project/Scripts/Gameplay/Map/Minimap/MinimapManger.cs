using System.Collections.Generic;
using UnityEngine;

public class MinimapManger : TGTHMonoBehaviour
{
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
        if (player == null) player = FindAnyObjectByType<Transform>();
    }
}
