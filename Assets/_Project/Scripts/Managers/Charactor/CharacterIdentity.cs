using UnityEngine;

public class CharacterIdentity : MonoBehaviour, ISaveable
{
    [SerializeField] private bool canLoadData = true;
    [Header("Cultivation")]
    public CultivationStage cultivationStage;

    [Header("Origin")]
    public RaceType raceType;
    public EssenceType essenceType;

    public StatsRaceData statsRaceData;
    public StatsCultivationPathData statsCultivationPathData;
    public StatsRealmData statsRealmData;
    public void Setup(StatsCultivationPathData statsCultivationPathData, StatsRealmData statsRealmData, StatsRaceData statsRaceData)
    {
        this.statsCultivationPathData = statsCultivationPathData;
        this.statsRealmData = statsRealmData;
        this.statsRaceData = statsRaceData;
    }
    public void LoadData(GameData _data)
    {
        if (!canLoadData) return;
        statsRaceData = _data.statsRaceData;
        statsCultivationPathData = _data.statsCultivationPathData;
        statsRealmData = _data.statsRealmData;

        cultivationStage = statsRealmData.cultivationStage;
        essenceType = statsCultivationPathData.essenceType;
        raceType = statsRaceData.raceType;
    }

    public void SaveGame(ref GameData _data)
    {
        
    }
}
