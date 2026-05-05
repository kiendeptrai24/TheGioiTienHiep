

using System;
using System.Collections.Generic;
using UnityEngine;

public class LoadEquipmentDataServerSide : ILoadGameData<GameDataCenter, AllGameDataResponseDto>
{
    public void LoadGameData(GameDataCenter gameData, AllGameDataResponseDto allGameData)
    {
        try
        {
            List<ItemDataDto> allItem = allGameData.equipmentRes;
            if (allItem == null)
            {
                Debug.Log("LoadGame: equipmentRes is null");
                return;
            }

            List<ItemData> itemDatas = new();
            List<EquipmentData> equipmentDatas = new();
            List<TechniqueData> techniqueDatas = new();
            List<SkillData> skillDatas = new();
            for (int i = 0; i < allItem.Count; i++)
            {
                var itemDto = allItem[i];
                ItemData itemData = DataMapper.MapItemData(itemDto);
                if (itemData != null)
                {
                    itemDatas.Add(itemData);
                    if (itemData is EquipmentData equip)
                        equipmentDatas.Add(equip);
                    else if (itemData is SkillData skill)
                        skillDatas.Add(skill);
                    else if (itemData is TechniqueData tech)
                        techniqueDatas.Add(tech);
                }
            }
            gameData.equipmentItems = equipmentDatas;
            gameData.skillItems = skillDatas;
            gameData.techniqueDatasItems = techniqueDatas;
            gameData.allItems.AddRange(itemDatas);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("LoadGame: Failed to load equipment data " + ex.Message);
        }
    }
}