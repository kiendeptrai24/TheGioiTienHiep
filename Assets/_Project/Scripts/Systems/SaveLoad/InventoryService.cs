
using System;

public class InventoryService : ISaveLoadRemote
{
    private PlayFabLogin playFabLogin;
    public InventoryService(PlayFabLogin playFabLogin)
    {
        this.playFabLogin = playFabLogin;
    }

    public void LoadGame(GameData gameData, Action callback)
    {
        playFabLogin.player.LoadData((gameDataDTO) =>
        {
            var itemsShop = new PlayerDataDTO();
            itemsShop = gameDataDTO;

            var iconLoader = AddressableLoader.Instance.GetLoader<IconLoader>(AddressableLoaderType.Sprite.ToString());
            var prefabLoader = AddressableLoader.Instance.GetLoader<PrefabLoader>(AddressableLoaderType.Prefab.ToString());
            var SODataBase = ScriptableObjectLoader.Instance;

            for (int i = 0; i < itemsShop.inventoryItems.Count; i++)
            {
                var item = itemsShop.inventoryItems[i];
                var itemData = SODataBase.GetItem(item.itemId);
                if (itemData == null)
                    continue;
                var sprite = iconLoader.Get(item.itemIconPath);
                itemData.itemIcon = sprite;

                if (itemData is HeroData heroData)
                {
                    SetHeroData(itemsShop, iconLoader, prefabLoader, SODataBase, i, itemData, heroData);
                    continue;
                }

                if (itemData is SkillData skillDatas)
                {
                    SetSkilldata(itemsShop, iconLoader, prefabLoader, i, skillDatas);
                    continue;
                }

                itemsShop.inventoryItems[i] = itemData;
            }
            gameData.allItemsDatas = itemsShop.inventoryItems;
            callback?.Invoke();
        });
    }

    private void SetHeroData(PlayerDataDTO itemsShop, IconLoader iconLoader, PrefabLoader prefabLoader, ScriptableObjectLoader SODataBase, int i, ItemData itemData, HeroData heroData)
    {
        var heroPrefab = prefabLoader.Get(itemData.itemFilePath);
        heroData.heroPrefab = heroPrefab;

        for (int h = 0; h < heroData.skillDatas.Count; h++)
        {
            var skill = heroData.skillDatas[h];

            var skillData = SODataBase.GetItem(skill.itemId) as SkillData;
            if (skillData == null)
                continue;
            SetSkilldata(itemsShop, iconLoader, prefabLoader, h, skillData);
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

        itemsShop.inventoryItems[i] = heroData;
    }

    private void SetSkilldata(PlayerDataDTO itemsShop, IconLoader iconLoader, PrefabLoader prefabLoader, int i, SkillData skillDatas)
    {
        skillDatas.itemIcon = iconLoader.Get(skillDatas.itemIconPath);
        skillDatas.skillEffectPrefab = prefabLoader.Get(skillDatas.itemFilePath);

        itemsShop.inventoryItems[i] = skillDatas;
    }
    public void SaveGame()
    {

    }
}