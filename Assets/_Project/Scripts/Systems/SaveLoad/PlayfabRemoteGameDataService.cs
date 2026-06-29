using System;
using System.Collections.Generic;

public class PlayfabRemoteGameDataService
{
    private readonly PlayfabSessionState state;

    private PlayFabDataClientService dataClientService;
    private ItemCharacterService characterService;

    public PlayfabRemoteGameDataService(PlayfabSessionState state)
    {
        this.state = state;
    }

    public List<ItemData> GetCharactersData()
    {
        return state.GameData.itemCharacterDatas;
    }

    public void ConfigureRemoteServices()
    {
        state.LoadRemotes.Clear();
        state.SaveRemotes.Clear();

        dataClientService = new PlayFabDataClientService(state.ClientApi);

        var playerInventoryService = new PlayerInventoryService(dataClientService);
        characterService = new ItemCharacterService(dataClientService);
        var gameBaseCharacterService = new GameBaseCharacterService(dataClientService);

        state.LoadRemotes.Add(playerInventoryService);
        state.LoadRemotes.Add(gameBaseCharacterService);
        state.SaveRemotes.Add(playerInventoryService);
    }

    public void LoadCharacterSelectionData(Action<List<ItemData>> onLoaded)
    {
        if (characterService == null)
        {
            onLoaded?.Invoke(state.GameData.itemCharacterDatas);
            return;
        }

        characterService.LoadGame(state.GameData, () =>
        {
            state.SaveRemotes.Add(characterService);
            onLoaded?.Invoke(state.GameData.itemCharacterDatas);
        });
    }

    public void AddCharacter(ItemData itemCharacter, Action<List<ItemData>> onCharacterChanged)
    {
        var heroData = itemCharacter as HeroData;
        if (heroData == null)
        {
            UnityEngine.Debug.LogError("AddCharacter failed: itemCharacter is not HeroData");
            return;
        }

        if (characterService == null)
        {
            UnityEngine.Debug.LogError("AddCharacter failed: characterService is not configured");
            return;
        }

        PopulateNewCharacterData(itemCharacter, heroData);
        characterService.SaveGame(state.GameData);
        SaveGameData();
        onCharacterChanged?.Invoke(state.GameData.itemCharacterDatas);
    }

    public void PrepareCharacterLoad(string characterId)
    {
        state.GameData.ClearNotCharacterData();
        state.GameData.characterId = characterId;
    }

    public void LoadGameData(Action<GameData> onLoaded)
    {
        int total = state.LoadRemotes.Count;
        if (total == 0)
        {
            onLoaded?.Invoke(state.GameData);
            return;
        }

        int completed = 0;
        foreach (var loadRemote in state.LoadRemotes)
        {
            loadRemote.LoadGame(state.GameData, () =>
            {
                completed++;
                if (completed == total)
                {
                    onLoaded?.Invoke(state.GameData);
                }
            });
        }
    }

    public void SaveGameData(Action<bool> onCompleted = null)
    {
        if (state.SaveRemotes.Count == 0)
        {
            onCompleted?.Invoke(true);
            return;
        }

        int completed = 0;
        bool allSucceeded = true;

        foreach (var saveRemote in state.SaveRemotes)
        {
            saveRemote.SaveGame(state.GameData, success =>
            {
                completed++;
                allSucceeded &= success;

                if (completed == state.SaveRemotes.Count)
                {
                    onCompleted?.Invoke(allSucceeded);
                }
            });
        }
    }

    public void ClearRemoteCache()
    {
        state.LoadRemotes.Clear();
        state.SaveRemotes.Clear();
    }

    private void PopulateNewCharacterData(ItemData itemCharacter, HeroData heroData)
    {
        state.GameData.ClearNotCharacterData();
        heroData.isCharacter = true;
        state.GameData.createdAt = TimeUtils.GetCurrentTimeString();
        state.GameData.characterName = itemCharacter.itemName;
        state.GameData.characterId = heroData.characterId;
        state.GameData.coins = 1000000;
        state.GameData.itemDatas.Add(itemCharacter);
        state.GameData.itemCharacterDatas.Add(itemCharacter);

        state.GameData.potentialPoint = heroData.realmData.rewardPotentialPoint;
        state.GameData.skillPoint = heroData.realmData.rewardSkillPoint;

        var realmData = GameDataCenterManager.Instance.GetItemById(heroData.realmId) as RealmData;
        if (realmData != null)
        {
            state.GameData.currentHealth = (int)realmData.health;
            state.GameData.currentMana = (int)realmData.mana;
            state.GameData.currentSpirit = (int)realmData.spirit;
        }
    }
}
