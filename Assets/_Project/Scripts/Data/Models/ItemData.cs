

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
    public ulong itemPrice;
    public bool canStack;
    public string itemDescription;
    [JsonIgnore]
    public int currentstack;
    public RealmType realmType;
    public string realmId;
    public QualityType qualityType;
    public ElementType elementType;
    //Offensive Stats
    public float health;
    public float mana;
    public float spirit;
    public float physicalDamage;
    public float magicalDamage;
    public float spiritDamage;

    //Defensive Stats
    public float physicalDefense;
    public float magicalDefense;
    public float spiritDefense;
    [JsonIgnore]
    public int potentialPoints;

    public virtual ItemData Clone()
    {
        return (ItemData)this.MemberwiseClone();
    }

}