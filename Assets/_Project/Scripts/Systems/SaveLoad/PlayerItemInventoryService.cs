
using System;
using UnityEngine;

public class PlayerItemInventoryService : ISaveLoadRemote
{
    private PlayFabDataService service;
    public PlayerItemInventoryService(PlayFabDataService service)
    {
        this.service = service;
    }

    public void LoadGame(GameData gameData, Action callback)
    {
        service.LoadPlayerData(gameData.characterId, (gameDataDTO) =>
        {
            try
            {
                if (gameDataDTO == null)
                {
                    callback?.Invoke();
                    return;
                }
                var itemsData = new ItemDataDTO();
                itemsData = gameDataDTO;

                var iconLoader = AddressableLoader.Instance.GetLoader<IconLoader>(AddressableLoaderType.Sprite.ToString());
                var prefabLoader = AddressableLoader.Instance.GetLoader<PrefabLoader>(AddressableLoaderType.Prefab.ToString());
                var SODataBase = ScriptableObjectLoader.Instance;

                for (int i = 0; i < itemsData.inventoryItems.Count; i++)
                {
                    var item = itemsData.inventoryItems[i];
                    if (item == null)
                    {
                        Debug.Log("item is null");
                        return;
                    }
                    var itemData = SODataBase.GetItem(item.instanceId);
                    itemData.itemName = gameDataDTO.inventoryItems[i].itemName;
                    itemData.realmType = gameDataDTO.inventoryItems[i].realmType;
                    var sprite = iconLoader.Get(item.itemIconPath);
                    itemData.itemIcon = sprite;

                    if (itemData is HeroData)
                        continue;

                    if (itemData is SkillData skillDatas)
                    {
                        SetSkilldata(itemsData, iconLoader, prefabLoader, i, skillDatas);
                    }
                    itemsData.inventoryItems[i] = itemData;
                }
                gameData.itemDatas.AddRange(itemsData.inventoryItems);
                callback?.Invoke();
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error occurred while loading item data." + ex.Message);
            }
        });
    }

    private void SetSkilldata(ItemDataDTO itemsData, IconLoader iconLoader, PrefabLoader prefabLoader, int i, SkillData skillDatas)
    {
        skillDatas.itemIcon = iconLoader.Get(skillDatas.itemIconPath);
        skillDatas.skillEffectPrefab = prefabLoader.Get(skillDatas.itemFilePath);

        itemsData.inventoryItems[i] = skillDatas;
    }
    public void SaveGame(GameData gameData)
    {
        service.SetItemInventoryData(gameData);
    }
}