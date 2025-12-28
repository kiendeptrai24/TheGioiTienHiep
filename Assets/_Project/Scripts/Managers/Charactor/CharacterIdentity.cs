using UnityEngine;

public class CharacterIdentity : MonoBehaviour, ISaveable
{
    [Header("Cultivation")]
    public CultivationStage cultivationStage;

    [Header("Origin")]
    public RaceType raceType;
    public EssenceType essenceType;

    public StatsRaceData statsRaceData;
    public StatsCultivationPathData statsCultivationPathData;
    public StatsRealmData statsRealmData;

    public void LoadData(GameData _data)
    {
        statsRaceData = _data.statsRaceData;
        statsCultivationPathData = _data.statsCultivationPathData;
        statsRealmData = _data.statsRealmData;

        cultivationStage = statsRealmData.cultivationStage;
        essenceType = statsCultivationPathData.essenceType;
        raceType = statsRaceData.raceType;
        Debug.Log(cultivationStage.ToString()+ " " + essenceType.ToString() + " " + raceType.ToString());
    }

    public void SaveGame(ref GameData _data)
    {
        
    }
}
