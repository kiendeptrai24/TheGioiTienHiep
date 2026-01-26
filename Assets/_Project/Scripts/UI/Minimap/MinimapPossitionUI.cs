
using System;
using TMPro;
using UnityEngine;

public class MinimapPossitionUI : TGTHMonoBehaviour
{
    [SerializeField] private TextMeshProUGUI posTxt;
    private MapSpawn mapSpawn;
    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
        mapSpawn.posPlayer += OnPosPlayer;

    }

    private void OnPosPlayer(int xPos, int yPos)
    {
        posTxt.text = xPos.ToString() + ":" + yPos.ToString();
    }

    protected override void Start()
    {
        base.Start();
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
        mapSpawn = FindAnyObjectByType<MapSpawn>();
    }
}