using System.Collections.Generic;
using Newtonsoft.Json;

public class ItemJsonConverter
{
    public static List<ItemData> FromJson(string json)
    {
        HeroDataDTO itemDataDTO = JsonConvert.DeserializeObject<HeroDataDTO>(json);
        return Convert(itemDataDTO);
    }

    public static string ToJson(List<ItemData> itemDatas)
    {
        var itemDataDTO = new HeroDataDTO();
        foreach (var item in itemDatas)
        {
            itemDataDTO.inventoryItems.Add(item);
            if (item is HeroData heroData)
            {
                itemDataDTO.championsIndex.Add(heroData.championIndex);
            }
        }
        return JsonConvert.SerializeObject(itemDataDTO);
    }
    public static List<ItemData> Convert(HeroDataDTO heroDTO)
    {
        var itemDatas = new List<ItemData>();


        var iconLoader = AddressableLoader.Instance.GetLoader<IconLoader>(AddressableLoaderType.Sprite.ToString());
        var prefabLoader = AddressableLoader.Instance.GetLoader<PrefabLoader>(AddressableLoaderType.Prefab.ToString());
        var SODataBase = ScriptableObjectLoader.Instance;

        for (int i = 0; i < heroDTO.inventoryItems.Count; i++)
        {
            var item = heroDTO.inventoryItems[i];
            var itemData = SODataBase.GetItem(item.itemId);
            itemData.itemName = heroDTO.inventoryItems[i].itemName;
            if (itemData == null)
                continue;
            var sprite = iconLoader.Get(item.itemIconPath);
            itemData.itemIcon = sprite;

            if (itemData is HeroData heroData)
            {
                heroData.championIndex = heroDTO.championsIndex[i];
                SetHeroData(heroDTO, iconLoader, prefabLoader, SODataBase, i, itemData, heroData);
                continue;
            }

            if (itemData is SkillData skillDatas)
            {
                SetSkilldata(heroDTO, iconLoader, prefabLoader, i, skillDatas);
                continue;
            }

            heroDTO.inventoryItems[i] = itemData;
        }
        itemDatas = heroDTO.inventoryItems;
        return itemDatas;
    }

    private static void SetHeroData(ItemDataDTO itemsteam, IconLoader iconLoader, PrefabLoader prefabLoader, ScriptableObjectLoader SODataBase, int i, ItemData itemData, HeroData heroData)
    {
        var heroPrefab = prefabLoader.Get(itemData.itemFilePath);
        heroData.heroPrefab = heroPrefab;

        for (int h = 0; h < heroData.skillDatas.Count; h++)
        {
            var skill = heroData.skillDatas[h];

            var skillData = SODataBase.GetItem(skill.itemId) as SkillData;
            if (skillData == null)
                continue;
            skillData.itemIcon = iconLoader.Get(skillData.itemIconPath);
            skillData.skillEffectPrefab = prefabLoader.Get(skillData.itemFilePath);
            heroData.skillDatas[h] = skillData;
        }

        for (int s = 0; s < heroData.techniqueDatas.Count; s++)
        {
            var technique = heroData.techniqueDatas[s];
            var techniqueData = SODataBase.GetItem(technique.itemId) as TechniqueData;
            if (techniqueData == null)
                continue;
            heroData.techniqueDatas[s] = techniqueData;
        }

        itemsteam.inventoryItems[i] = heroData;
    }

    private static void SetSkilldata(ItemDataDTO itemsteam, IconLoader iconLoader, PrefabLoader prefabLoader, int i, SkillData skillDatas)
    {
        skillDatas.itemIcon = iconLoader.Get(skillDatas.itemIconPath);
        skillDatas.skillEffectPrefab = prefabLoader.Get(skillDatas.itemFilePath);

        itemsteam.inventoryItems[i] = skillDatas;
    }
}