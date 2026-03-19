using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public static class ItemJsonCreator
{
    public static void CreateItemJson(List<ItemData> itemList)
    {
        PlayerDataDTO itemTest = new PlayerDataDTO();
        itemTest.inventoryItems = itemList;
        string json = JsonConvert.SerializeObject(itemTest);

        string path = Application.dataPath + "/item.json";

        File.WriteAllText(path, json);

        Debug.Log("JSON created at: " + path);
        Debug.Log(json);
    }
}