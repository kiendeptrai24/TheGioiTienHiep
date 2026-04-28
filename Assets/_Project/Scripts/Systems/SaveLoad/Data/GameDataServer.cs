using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameDataServer
{
    public List<ItemData> allItems;
    public List<ItemData> raceAndEssenceItems;
    public List<ItemData> realmItems;
    public List<ItemData> championItems;
    public List<ItemShop> shopItems;

    public GameDataServer()
    {
        allItems = new List<ItemData>();
        raceAndEssenceItems = new List<ItemData>();
        realmItems = new List<ItemData>();
        championItems = new List<ItemData>();
        shopItems = new List<ItemShop>();
    }
}