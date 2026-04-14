

using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
[System.Serializable]
public class HeroData : ItemData
{
    [JsonIgnore]
    public bool isCharactor;
    public string characterId;
    [JsonIgnore]
    public StatsRealmData realmData;
    [JsonIgnore]
    public StatsCultivationPathData statsCultivationPathData;
    [JsonIgnore]
    public StatsRaceData raceData;
    public RaceType raceType;
    [JsonIgnore]
    public EssenceType essenceType;
    [JsonIgnore]
    public int level;
    [JsonIgnore]
    public float attackRange;
    [JsonIgnore]
    public float movementSpeed;
    [JsonIgnore]
    public float attackSpeed;
    [JsonIgnore]
    public float health; // persent
    [JsonIgnore]
    public float mana; // persent
    [JsonIgnore]
    public float spirit; // persent
    [JsonIgnore]
    public float healthRegen;
    [JsonIgnore]
    public float manaRegen;
    [JsonIgnore]
    public float spiritRegen;
    [JsonIgnore]
    public int physicalDamagePoint; // value
    [JsonIgnore]
    public int magicalDamagePoint; // value
    [JsonIgnore]
    public int spiritDamagePoint; // value
    [JsonIgnore]
    public int physicalDefensePoint; // value
    [JsonIgnore]
    public int magicalDefensePoint; // value
    [JsonIgnore]
    public int spiritDefensePoint; // value
    [JsonIgnore]
    public int healthPoint;
    [JsonIgnore]
    public int manaPoint;
    [JsonIgnore]
    public int spiritPoint;
    [JsonIgnore]
    public int moveSpeedPoint;
    [JsonIgnore]
    public int spititRangePoint;
    [JsonIgnore]
    public Vector2Int championIndex;
    public List<EquitmentData> equipmentDatas = new();
    public List<SkillData> skillDatas = new();
    public List<TechniqueData> techniqueDatas = new();
    public LevelUpConditionData levelUpConditionData = new();
    [JsonIgnore]
    public GameObject heroPrefab;

    public override ItemData Clone()
    {
        return (HeroData)this.MemberwiseClone();
    }
}