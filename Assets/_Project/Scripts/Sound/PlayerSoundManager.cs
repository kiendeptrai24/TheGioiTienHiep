using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerSoundManager : Singleton<PlayerSoundManager>
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer mainAudioMixer;

    [Header("Audio Source")]
    public AudioSource musicSource;
    public List<AudioClipData> sounds = new List<AudioClipData>();
    private Dictionary<string, AudioClipData> soundMap = new Dictionary<string, AudioClipData>();
    protected override void Awake()
    {
        base.Awake();
        musicSource.playOnAwake = false;
        SetUpSounds();
    }

    private void SetUpSounds()
    {
        foreach (var sound in sounds)
        {
            soundMap.Add(sound.id, sound);
        }
    }

    protected override void Start()
    {
        base.Start();
        UpdateOnload();
    }

    public void StopMusic()
    {
        if (musicSource.isPlaying)
            musicSource.Stop();
    }
    public AudioClip GetSound(string id)
    {
        AudioClipData data = soundMap[id];
        if (data == null) return null;
        return data.clip;
    }
    #region Save / Load & Volume Logic (Tuyến tính 0 -> 1)

    private void UpdateOnload()
    {
        float savedMaster = PlayerPrefs.GetFloat(GameConstantsUtils.MASTER_VOL_PARAM, 1f);
        SetSfxVolume(savedMaster);
    }
    public void PlayClip(string id, bool loop = false)
    {
        var clip = GetSound(id);
        if (clip == null) return;
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
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
