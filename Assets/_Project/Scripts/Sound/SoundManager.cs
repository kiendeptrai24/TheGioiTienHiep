using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Audio;

public class SoundManager : Singleton<SoundManager>
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer mainAudioMixer;

    [Header("Audio Source")]
    public AudioSource musicSource;

    [Header("Music Playlist")]
    public List<AudioClip> playlist = new List<AudioClip>();
    private int currentTrackIndex = 0;

    [Header("Options")]
    public bool loopPlaylist = true;
    public bool autoPlayNext = true;

    protected override void Start()
    {
        base.Start();
        UpdateOnload();

        if (playlist.Count > 0)
            PlayTrack(0);
    }

    private void Update()
    {
        if (autoPlayNext && !musicSource.isPlaying && playlist.Count > 0)
            NextTrack();
    }

    public void PlayTrack(int index)
    {
        if (index < 0 || index >= playlist.Count) return;
        currentTrackIndex = index;
        musicSource.clip = playlist[index];
        musicSource.Play();
        Debug.Log($"▶ Now Playing: {playlist[index].name}");
    }

    public void NextTrack()
    {
        currentTrackIndex++;
        if (currentTrackIndex >= playlist.Count)
        {
            if (loopPlaylist) currentTrackIndex = 0;
            else return;
        }
        PlayTrack(currentTrackIndex);
    }

    public void PreviousTrack()
    {
        currentTrackIndex--;
        if (currentTrackIndex < 0)
        {
            if (loopPlaylist) currentTrackIndex = playlist.Count - 1;
            else return;
        }
        PlayTrack(currentTrackIndex);
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public string GetCurrentTrackName()
    {
        return musicSource.clip != null ? musicSource.clip.name : "None";
    }

    #region Save / Load & Volume Logic (Tuyến tính 0 -> 1)

    private void UpdateOnload()
    {
        float savedMaster = PlayerPrefs.GetFloat(GameConstantsUtils.MASTER_VOL_PARAM, 1f);
        float savedMusic = PlayerPrefs.GetFloat(GameConstantsUtils.MUSIC_VOL_PARAM, 1f);
        Debug.Log(savedMaster + " " + savedMusic);
        ApplyVolumeToMixer(GameConstantsUtils.MASTER_VOL_PARAM, savedMaster);
        ApplyVolumeToMixer(GameConstantsUtils.MUSIC_VOL_PARAM, savedMusic);
    }

    private void ApplyVolumeToMixer(string parameterName, float sliderValue)
    {
        if (mainAudioMixer == null) return;
        // Chuyển đổi sang Decibel để cấu hình Mixer
        if (sliderValue <= 0) sliderValue = 0.0001f;
        float decibelValue = Mathf.Log10(sliderValue / 10) * 20;
        mainAudioMixer.SetFloat(parameterName, decibelValue);
    }
    #endregion
}