[System.Serializable]
public class AudioSetupData : IPopupData
{
    public float masterVolume;
    public float musicVolume;
    public float sfxVolume;

    public AudioSetupData(float masterVolume, float musicVolume, float sfxVolume)
    {
        this.masterVolume = masterVolume;
        this.musicVolume = musicVolume;
        this.sfxVolume = sfxVolume;
    }
    public AudioSetupData()
    {
    }
}