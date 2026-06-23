using System;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OpptionsPopup : BasePopup<AudioSetupData, StatsPointPopupData>
{
    [Header("--- UI Buttons ---")]
    [SerializeField] private Button cancelBtn;

    [Header("--- Audio Mixer (Single) ---")]
    [Tooltip("Chỉ cần kéo 1 Audio Mixer duy nhất quản lý chung vào đây")]
    [SerializeField] private AudioMixer mainAudioMixer;

    [Header("--- Sliders ---")]
    [SerializeField] private Slider totalVolumeSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider graphicsSlider;

    protected override void Awake()
    {
        base.Awake();
        // Đăng ký sự kiện lắng nghe khi kéo slider
        totalVolumeSlider.onValueChanged.AddListener(OnTotalVolumeChanged);
        musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
        graphicsSlider.onValueChanged.AddListener(OnGraphicsVolumeChanged);
    }
    protected override void Start()
    {
        base.Start();
        LoadAudioSettings();
    }
    public override void Show()
    {
        base.Show();
        // Load lại cấu hình cũ ngay khi Popup hiển thị lên màn hình
    }

    private void LoadAudioSettings()
    {
        // Khởi tạo slider và áp dụng volume đã lưu (mặc định là 1f nếu chưa có)
        float totalVol = PlayerPrefs.GetFloat(GameConstantsUtils.MASTER_VOL_PARAM, 1f);
        totalVolumeSlider.value = totalVol;
        UpdateMixerVolume(GameConstantsUtils.MASTER_VOL_PARAM, totalVol);

        float musicVol = PlayerPrefs.GetFloat(GameConstantsUtils.MUSIC_VOL_PARAM, 1f);
        musicSlider.value = musicVol;
        UpdateMixerVolume(GameConstantsUtils.MUSIC_VOL_PARAM, musicVol);

        float sfxVol = PlayerPrefs.GetFloat(GameConstantsUtils.SFX_VOL_PARAM, 1f);
        sfxSlider.value = sfxVol;
        UpdateMixerVolume(GameConstantsUtils.SFX_VOL_PARAM, sfxVol);
        Debug.Log(totalVol + " " + musicVol + " " + sfxVol);

        // Khởi tạo đồ họa (ví dụ dùng QualitySettings của Unity, sliderValue đại diện cho Quality Level)
        if (graphicsSlider != null)
        {
            int savedGraphics = PlayerPrefs.GetInt(GameConstantsUtils.GRAPHICS_PREF_KEY, QualitySettings.GetQualityLevel());
            graphicsSlider.value = savedGraphics;
        }
    }

    #region Slider Events
    private void OnTotalVolumeChanged(float value)
    {
        UpdateMixerVolume(GameConstantsUtils.MASTER_VOL_PARAM, value);
    }

    private void OnMusicVolumeChanged(float value)
    {
        UpdateMixerVolume(GameConstantsUtils.MUSIC_VOL_PARAM, value);
    }

    private void OnSfxVolumeChanged(float value)
    {
        UpdateMixerVolume(GameConstantsUtils.SFX_VOL_PARAM, value);
    }

    private void OnGraphicsVolumeChanged(float value)
    {
        // Chuyển float từ slider thành int cho Quality Level
        int qualityLevel = Mathf.RoundToInt(value);
        QualitySettings.SetQualityLevel(qualityLevel, true);
        PlayerPrefs.SetInt(GameConstantsUtils.GRAPHICS_PREF_KEY, qualityLevel);
    }
    #endregion

    /// <summary>
    /// Hàm lõi xử lý tính toán Log10 chuyển đổi sang Decibel và lưu trữ dữ liệu
    /// </summary>
    private void UpdateMixerVolume(string parameterName, float sliderValue)
    {
        if (mainAudioMixer == null) return;

        // Tránh lỗi toán học log10(0) bằng cách tiệm cận giá trị nhỏ nhất
        if (sliderValue <= 0) sliderValue = 0.0001f;

        // Khi sliderValue = 10 -> 10/10 = 1 -> Log10(1) * 20 = 0 dB (Max)
        float decibelValue = Mathf.Log10(sliderValue / 10f) * 20;

        mainAudioMixer.SetFloat(parameterName, decibelValue);
        PlayerPrefs.SetFloat(parameterName, sliderValue);
    }
    protected override void SetupButtons()
    {
        base.SetupButtons();
        cancelBtn.onClick.AddListener(OnCancelClicked);
    }
    private void OnCancelClicked()
    {
        m_EffectManager?.PlayOneShot("button-click");
        Hide();
    }

    public override void Hide()
    {
        base.Hide();
        PlayerPrefs.Save();
    }

    protected override StatsPointPopupData GetResult() => null;
    protected override void LoadComponent() => base.LoadComponent();
    protected override void SetupPopupData(AudioSetupData data)
    {

    }
}