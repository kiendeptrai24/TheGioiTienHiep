

using Newtonsoft.Json;
using UnityEngine;
[System.Serializable]
public class ItemData
{
    public string instanceId;
    public string itemId;
    public ItemType itemType;
    public string itemName;
    [JsonIgnore]
    public Sprite itemIcon;
    public string itemIconPath;
    public string itemFilePath;
    [JsonIgnore]
    public int itemPrice;
    [JsonIgnore]
    public bool canStack;
    [JsonIgnore]
    public string itemDescription;
    [JsonIgnore]
    public int currentstack;
    public RealmType realmType;
    [JsonIgnore]
    public QualityType qualityType;
    [JsonIgnore]
    public ElementType elementType;
    //Offensive Stats

    [JsonIgnore]
    public float physicalDamage;
    [JsonIgnore]
    public float magicalDamage;
    [JsonIgnore]
    public float spiritDamage;

    //Defensive Stats
    [JsonIgnore]
    public float physicalDefense;
    [JsonIgnore]
    public float magicalDefense;
    [JsonIgnore]
    public float spiritDefense;
    public virtual ItemData Clone()
    {
        return (ItemData)this.MemberwiseClone();
    }

}