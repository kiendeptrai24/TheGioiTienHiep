

using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
[System.Serializable]
public class HeroData : ItemData
{
    [JsonIgnore]
    public bool isCharactor;
    [JsonIgnore]
    public StatsRealmData statsRealmData;
    [JsonIgnore]
    public StatsCultivationPathData statsCultivationPathData;
    [JsonIgnore]
    public StatsRaceData statsRaceData;
    [JsonIgnore]
    public RaceType raceType;
    [JsonIgnore]
    public EssenceType essenceType;
    [JsonIgnore]
    public int level;
    [JsonIgnore]
    public float attackRange;
    [JsonIgnore]
    public float moveSpeed;
    [JsonIgnore]
    public float health; // persent
    [JsonIgnore]
    public float mana; // persent
    [JsonIgnore]
    public float spirit; // persent
    [JsonIgnore]
    public float physicalDamagePoint; // value
    [JsonIgnore]
    public float magicalDamagePoint; // value
    [JsonIgnore]
    public float spiritDamagePoint; // value
    [JsonIgnore]
    public float physicalDefensePoint; // value
    [JsonIgnore]
    public float magicalDefensePoint; // value
    [JsonIgnore]
    public float spiritDefensePoint; // value
    [JsonIgnore]
    public Vector2Int championIndex;
    public List<EquitmentData> equitmentDatas = new();
    public List<SkillData> skillDatas = new();
    public List<TechniqueData> techniqueDatas = new();
    [JsonIgnore]
    public GameObject heroPrefab;
}