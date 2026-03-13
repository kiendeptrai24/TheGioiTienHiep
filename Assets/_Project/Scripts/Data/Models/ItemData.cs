

using UnityEngine;
[System.Serializable]
public class ItemData
{
    public string itemId;
    public ItemType itemType;
    public string itemName;
    public Sprite itemIcon;
    public int itemPrice;
    public bool canStack;
    public string itemDescription;
    public int currentstack;
    public RealmType realmType;
    public QualityType qualityType;
    public ElementType elementType;

    //Offensive Stats
    public float physicalDamage;
    public float magicalDamage;
    public float spiritDamage;

    //Defensive Stats
    public float physicalDefense;
    public float magicalDefense;
    public float spiritDefense;
    public ItemData Clone()
    {
        return (ItemData)this.MemberwiseClone();
    }

}