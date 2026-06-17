using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public class AudioClipData
{
    public string id;
    public AudioClip clip;
}
public class EffectManager : Singleton<EffectManager>
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer mainAudioMixer;

    [Header("Audio Source")]
    [SerializeField] private AudioSource effectSource;

    public AudioClip currentSound;
    public List<AudioClipData> sounds = new List<AudioClipData>();
    private Dictionary<string, AudioClipData> soundMap = new Dictionary<string, AudioClipData>();


    protected override void Awake()
    {
        base.Awake();
        effectSource.playOnAwake = false;
        SetUpSounds();
    }

    protected override void Start()
    {
        base.Start();
        Load();
    }
    public void SetUpSounds()
    {
        foreach (var sound in sounds)
        {
            soundMap.Add(sound.id, sound);
        }
    }
    public void PlayOneShot(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        effectSource.PlayOneShot(clip, Mathf.Clamp01(volume));
    }
    public void PlayOneShot(string id, float volume = 1f)
    {
        AudioClipData data = soundMap[id];
        if (data == null) return;
        PlayOneShot(data.clip, volume);
    }
    #region Save / Load & Volume Logic

    private void Load()
    {
        float savedSfx = PlayerPrefs.GetFloat(GameConstantsUtils.SFX_VOL_PARAM, 1f);
        SetSfxVolume(savedSfx);
    }

    public void SetSfxVolume(float sliderValue)
    {
        if (mainAudioMixer == null) return;

        if (sliderValue <= 0) sliderValue = 0.0001f;
        float decibelValue = Mathf.Log10(sliderValue / 10) * 20;
        mainAudioMixer.SetFloat(GameConstantsUtils.SFX_VOL_PARAM, decibelValue);
    }
    #endregion
}