using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameDataServer
{
    public List<ItemData> allItems;
    public List<ItemData> raceAndEssenceItems;
    public List<ItemData> realmItems;
    public List<ItemData> shopItems;

    public GameDataServer()
    {
        allItems = new List<ItemData>();
    }
}