using System;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class StatsRealmData : ItemData
{
    //Cultivation Realm
    [JsonIgnore]
    public string description;
    //Resources
    [JsonIgnore]
    public int maxHealth;
    [JsonIgnore]
    public int maxMana;
    [JsonIgnore]
    public int maxSpirit;

    //Offensive Stats
    [JsonIgnore]
    public int critRate;
    [JsonIgnore]
    public int critDamage;

    //Defensive Stats
    [JsonIgnore]
    public int evasion;
    [JsonIgnore]
    public int armorPenetration;

    //Speed Stats
    [JsonIgnore]
    public int movementSpeed;
    [JsonIgnore]
    public int attackSpeed;
    [JsonIgnore]
    public int castSpeed;

    //Progression Stats
    [JsonIgnore]
    public int potential;
    [JsonIgnore]
    public int skillPoints;
    [JsonIgnore]
    public int combatPower;
    //Critical Stats
    [JsonIgnore]
    public int spiritRange;
    [Header("Upgrade Materials")]
    [JsonIgnore]
    public float powerCost;              // Power
    [JsonIgnore]
    public int linhThaoCost;              // Linh thảo
    [JsonIgnore]
    public int khoangThachCost;            // Khoáng thạch
    [JsonIgnore]
    public int yeuDanCost;          // Yêu đan
    [JsonIgnore]
    public int maHachCost;          // Ma hạch
    [JsonIgnore]
    public int linhThachCost;        // Linh thạch
    [JsonIgnore]
    public int rewardPotentialPoint;
    [JsonIgnore]
    public int rewardSkillPoint;
}