using System;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class RealmData : ItemData
{
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
    public int spiritRange;
    [Header("Upgrade Materials")]
    [JsonIgnore]
    public float powerCost;              // Power
    public int linhThaoCost;              // Linh thảo
    public int khoangThachCost;            // Khoáng thạch
    public int yeuDanCost;          // Yêu đan
    public int maHachCost;          // Ma hạch
    public int linhThachCost;        // Linh thạch
    public int trucCoDanCost;        // Yêu đan
    public string itemsCost;
    public int rewardPotentialPoint;
    public int rewardSkillPoint;
    public float rate;
    public float increaseRate;
    public long timeSeconds;
    override public ItemData Clone()
    {
        return (RealmData)this.MemberwiseClone();
    }
}