using System;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OpptionsPopup : BasePopup<BaseSetupData, StatsPointPopupData>
{
    [SerializeField] private Button cancelBtn;
    [SerializeField] private Slider totalVolumeSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider graphicsSlider;
    protected override void Awake()
    {
        base.Awake();
        totalVolumeSlider.onValueChanged.AddListener(OnTotalVolumeChanged);
        musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
        graphicsSlider.onValueChanged.AddListener(OnGraphicsVolumeChanged);
    }

    private void OnGraphicsVolumeChanged(float arg0)
    {
        
    }

    private void OnSfxVolumeChanged(float arg0)
    {
        
    }

    private void OnMusicVolumeChanged(float arg0)
    {
        
    }

    private void OnTotalVolumeChanged(float arg0)
    {
        
    }

    public override void Show()
    {
        base.Show();
    }
    protected override void SetupButtons()
    {
        base.SetupButtons();
        cancelBtn.onClick.AddListener(OnCancelClicked);
    }
    public override void Hide()
    {
        base.Hide();
    }

    protected override StatsPointPopupData GetResult()
    {
        return null;
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
    }
    protected override void SetupPopupData(BaseSetupData data)
    {

    }
}