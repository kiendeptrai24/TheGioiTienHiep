

using UnityEngine;
[System.Serializable]
public class ItemData  
{
    public string itemId;
    public ItemType itemType;
    public string itemName;
    public Sprite itemIcon;
    public string itemDescription;
    public int currentstack;
   // public CultivationStage cultivationStage;
    public QualityType qualityType;

    
    //Offensive Stats
    public float physicalDamage;
    public float magicalDamage;
    public float spiritDamage;

    //Defensive Stats
    public float physicalDefense;
    public float magicalDefense;
    public float spiritDefense;
}