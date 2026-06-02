

using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
[System.Serializable]
public class HeroData : ItemData
{
    public bool isCharacter;
    public string characterId;
    [JsonIgnore]
    public RealmData realmData;
    public string raceId;
    public RaceType raceType;
    [JsonIgnore]
    public RaceData raceData;
    public string essenceId;
    public EssenceType essenceType;
    [JsonIgnore]
    public EssenceData essenceData;
    [JsonIgnore]
    public int level;
    public int attackRange;
    public float movementSpeed;
    public float attackSpeed;
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
    public int spiritRangePoint;
    [JsonIgnore]
    public Vector2Int championIndex;
    public List<string> equipmentIds = new();
    public List<EquipmentData> equipmentDatas = new();
    public List<string> skillIds = new();
    public List<SkillData> skillDatas = new();
    public List<string> techniqueIds = new();
    public List<TechniqueData> techniqueDatas = new();
    public LevelUpConditionData levelUpConditionData = new();
    [JsonIgnore]
    public GameObject heroPrefab;
    [JsonIgnore]
    public int potentialPoint;
    [JsonIgnore]
    public int skillPoint;
    [JsonIgnore]
    public float healthBonus;
    [JsonIgnore]
    public float manaBonus;
    [JsonIgnore]
    public float spiritBonus;
    [JsonIgnore]
    public float physicalDamageBonus;
    [JsonIgnore]
    public float magicalDamageBonus;
    [JsonIgnore]
    public float spiritDamageBonus;
    [JsonIgnore]
    public float physicalDefenseBonus;
    [JsonIgnore]
    public float spiritDefenseBonus;

    public override ItemData Clone()
    {
        return (HeroData)this.MemberwiseClone();
    }
}