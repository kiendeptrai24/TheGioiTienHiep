using System;
using System.Collections.Generic;
using UnityEngine;

public static class DataMapper
{
    public static HeroData MapChampionData(ChampionDataDto dto)
    {
        if (dto == null) return null;
        bool isCharacter = dto.isCharacter.HasValue && dto.isCharacter.Value;
        HeroData heroData = new HeroData();
        heroData.instanceId = dto.instanceId;
        heroData.itemName = dto.name;
        heroData.itemDescription = dto.description;
        heroData.qualityType = dto.quality;
        heroData.raceId = dto.raceId;
        heroData.realmId = dto.realmId;
        heroData.essenceId = dto.essenceId;
        heroData.elementType = dto.elementType;
        heroData.attackRange = dto.attackRange;
        heroData.isCharactor = isCharacter;

        heroData.healthPoint = dto.healthPoint;
        heroData.manaPoint = dto.manaPoint;
        heroData.spiritPoint = dto.spiritPoint;

        heroData.physicalDamagePoint = dto.physicalDamagePoint;
        heroData.magicalDamagePoint = dto.magicalDamagePoint;
        heroData.spiritDamagePoint = dto.spiritDamagePoint;

        heroData.physicalDefensePoint = dto.physicalDefensePoint;
        heroData.magicalDefensePoint = dto.magicalDefensePoint;
        heroData.spiritDefensePoint = dto.spiritDefensePoint;

        heroData.healthBonus = DataParseUtils.ParseNumberOrPercent(dto.healthBonus);
        heroData.manaBonus = DataParseUtils.ParseNumberOrPercent(dto.manaBonus);
        heroData.spiritBonus = DataParseUtils.ParseNumberOrPercent(dto.spiritBonus);

        heroData.physicalDamageBonus = DataParseUtils.ParseNumberOrPercent(dto.physicalDamageBonus);
        heroData.magicalDamageBonus = DataParseUtils.ParseNumberOrPercent(dto.magicalDamageBonus);
        heroData.spiritDamageBonus = DataParseUtils.ParseNumberOrPercent(dto.spiritDamageBonus);

        heroData.physicalDefenseBonus = DataParseUtils.ParseNumberOrPercent(dto.physicalDefenseBonus);
        heroData.magicalDamageBonus = DataParseUtils.ParseNumberOrPercent(dto.magicalDefenseBonus);
        heroData.spiritDefenseBonus = DataParseUtils.ParseNumberOrPercent(dto.spiritDefenseBonus);
        heroData.techniqueIds = dto.techniqueIds ?? new();
        heroData.skillIds = dto.skillIds ?? new();
        heroData.equipmentIds = dto.equipmentIds ?? new();
        return heroData;
    }

    public static HeroData MapCharacterData(CharacterDataDto dto)
    {
        if (dto == null) return null;
        HeroData heroData = new HeroData();
        heroData.instanceId = dto.instanceId;
        heroData.itemDescription = dto.description;
        heroData.raceId = dto.raceId;
        heroData.realmId = dto.realmId;
        heroData.isCharactor = true;
        if (!string.IsNullOrEmpty(dto.essenceId))
        {
            heroData.essenceId = dto.essenceId;
        }
        return heroData;
    }

    public static ItemData MapItemData(ItemDataDto dto)
    {
        if (dto == null) return null;

        ItemData itemData;
        if (dto.itemType == ItemType.Equipment)
        {
            var equipData = new EquipmentData();
            equipData.raceType = dto.raceType;
            if (dto.equipmentType.HasValue)
                equipData.equipmentType = dto.equipmentType.Value;
            itemData = equipData;
        }
        else if (dto.itemType == ItemType.Skill)
        {
            var skillData = new SkillData();
            skillData.raceType = dto.raceType;
            if (dto.skillType.HasValue)
                skillData.skillType = dto.skillType.Value;
            itemData = skillData;
        }
        else if (dto.itemType == ItemType.Technique)
        {
            var techniqueData = new TechniqueData();
            techniqueData.raceType = dto.raceType;
            if (dto.techniqueType.HasValue)
                techniqueData.techniqueType = dto.techniqueType.Value;
            itemData = techniqueData;
        }
        else
        {
            itemData = new ItemData();
        }

        itemData.instanceId = dto.instanceId;
        itemData.itemName = dto.itemName;
        itemData.itemDescription = dto.description;
        itemData.itemType = dto.itemType;
        itemData.qualityType = dto.qualityType;
        itemData.realmType = dto.realmType;
        itemData.health = DataParseUtils.ParseNumberOrPercent(dto.health);
        itemData.mana = DataParseUtils.ParseNumberOrPercent(dto.mana);
        itemData.spirit = DataParseUtils.ParseNumberOrPercent(dto.spirit);
        itemData.physicalDamage = DataParseUtils.ParseNumberOrPercent(dto.physicalDamage);
        itemData.magicalDamage = DataParseUtils.ParseNumberOrPercent(dto.magicalDamage);
        itemData.spiritDamage = DataParseUtils.ParseNumberOrPercent(dto.spiritDamage);

        itemData.physicalDefense = DataParseUtils.ParseNumberOrPercent(dto.physicalDefense);
        itemData.magicalDefense = DataParseUtils.ParseNumberOrPercent(dto.magicalDefense);
        itemData.spiritDefense = DataParseUtils.ParseNumberOrPercent(dto.spiritDefense);

        itemData.potentialPoints = dto.potentialPoints;

        return itemData;
    }

    public static EssenceData MapEssenceData(EssenceAndRaceDataDto dto)
    {
        if (dto == null || dto.type != EssenceAndRaceType.Essence) return null;

        EssenceData essenceData = new EssenceData();
        essenceData.instanceId = dto.instanceId;
        essenceData.itemName = dto.instanceId; // As per original code
        essenceData.itemDescription = dto.instanceId; // As per original code

        if (dto.essenceType.HasValue)
            essenceData.essenceType = dto.essenceType.Value;

        essenceData.physicalDamagePoint = (int)DataParseUtils.ParseNumberOrPercent(dto.physicalDamagePoint);
        essenceData.magicalDamagePoint = (int)DataParseUtils.ParseNumberOrPercent(dto.magicalDamagePoint);
        essenceData.spiritDamagePoint = (int)DataParseUtils.ParseNumberOrPercent(dto.spiritDamagePoint);
        essenceData.physicalDefensePoint = (int)DataParseUtils.ParseNumberOrPercent(dto.physicalDefensePoint);
        essenceData.magicalDefensePoint = (int)DataParseUtils.ParseNumberOrPercent(dto.magicalDefensePoint);
        essenceData.spiritDefensePoint = (int)DataParseUtils.ParseNumberOrPercent(dto.spiritDefensePoint);
        essenceData.healthPoint = (int)DataParseUtils.ParseNumberOrPercent(dto.healthPoint);
        essenceData.manaPoint = (int)DataParseUtils.ParseNumberOrPercent(dto.manaPoint);
        essenceData.spiritPoint = (int)DataParseUtils.ParseNumberOrPercent(dto.spiritPoint);
        essenceData.spiritRangePoint = (int)DataParseUtils.ParseNumberOrPercent(dto.spiritRangePoint);
        essenceData.movementSpeedPoint = (int)DataParseUtils.ParseNumberOrPercent(dto.movementSpeedPoint);

        return essenceData;
    }

    public static RaceData MapRaceData(EssenceAndRaceDataDto dto)
    {
        if (dto == null || dto.type != EssenceAndRaceType.Race) return null;

        RaceData raceData = new RaceData();
        raceData.instanceId = dto.instanceId;
        raceData.itemName = dto.instanceId; // As per original code

        if (dto.raceType.HasValue)
            raceData.raceType = dto.raceType.Value;

        raceData.healthPoint = DataParseUtils.ParseNumberOrPercent(dto.healthPoint);
        raceData.manaPoint = DataParseUtils.ParseNumberOrPercent(dto.manaPoint);
        raceData.spiritPoint = DataParseUtils.ParseNumberOrPercent(dto.spiritPoint);
        raceData.physicalDamagePoint = DataParseUtils.ParseNumberOrPercent(dto.physicalDamagePoint);
        raceData.magicalDamagePoint = DataParseUtils.ParseNumberOrPercent(dto.magicalDamagePoint);
        raceData.spiritDamagePoint = DataParseUtils.ParseNumberOrPercent(dto.spiritDamagePoint);
        raceData.physicalDefensePoint = DataParseUtils.ParseNumberOrPercent(dto.physicalDefensePoint);
        raceData.magicalDefensePoint = DataParseUtils.ParseNumberOrPercent(dto.magicalDefensePoint);
        raceData.spiritDefensePoint = DataParseUtils.ParseNumberOrPercent(dto.spiritDefensePoint);

        return raceData;
    }

    public static RealmData MapRealmData(ItemRealmDataDto dto)
    {
        if (dto == null) return null;

        RealmData realmData = new RealmData();
        realmData.instanceId = dto.instanceId;
        realmData.realmId = dto.instanceId;
        realmData.realmType = dto.realmType;
        realmData.health = dto.health;
        realmData.mana = dto.mana;
        realmData.spirit = dto.spirit;
        realmData.physicalDamage = dto.physicalDamage;
        realmData.magicalDamage = dto.magicalDamage;
        realmData.spiritDamage = dto.spiritDamage;
        realmData.physicalDefense = dto.physicalDefense;
        realmData.magicalDefense = dto.magicalDefense;
        realmData.spiritDefense = dto.spiritDefense;
        realmData.spiritRange = dto.spiritCritRate;
        realmData.movementSpeed = dto.movementSpeed;
        realmData.rewardPotentialPoint = dto.potentialPoints;
        realmData.rewardSkillPoint = dto.skillPoints;
        realmData.lthao = dto.lthao;
        realmData.item = dto.item == null ? "" : dto.item;
        realmData.rate = DataParseUtils.ParsePercent(dto.rate);
        realmData.increaseRate = DataParseUtils.ParsePercent(dto.increaseRate);
        realmData.timeSeconds = DataParseUtils.ParseTimeToSeconds(dto.time);

        return realmData;
    }

    public static ItemData MapShopData(ShopDataDto dto)
    {
        if (dto == null) return null;

        ItemData itemData = new ItemData();
        itemData.instanceId = dto.instanceId;
        itemData.itemPrice = dto.price;

        return itemData;
    }

    public static SpiritStoneMineData MapSpiritStoneMineData(SpiritStoneMineDataDto dto)
    {
        if (dto == null) return null;
        SpiritStoneMineData mineData = new SpiritStoneMineData();
        mineData.instanceId = dto.instanceId;
        mineData.itemName = dto.name;
        mineData.level = dto.level;
        mineData.maxStorage = dto.amount;
        mineData.yieldPerHarvest = dto.yieldPerHarvest;
        mineData.miningTime = 1;
        return mineData;

    }
    public static DemonBeastData MapDemonBeastData(DemonBeastDataDto dto)
    {
        if (dto == null) return null;
        DemonBeastData beastData = new DemonBeastData();
        beastData.instanceId = dto.instanceId;
        beastData.itemName = dto.name;
        beastData.itemDescription = dto.description;
        beastData.level = dto.level;
        beastData.lthach = dto.lthach;
        return beastData;
    }

    public static ChampionDataDto ToDto(HeroData data)
    {
        if (data == null) return null;
        bool isCharacter = data.isCharactor;
        return new ChampionDataDto
        {
            instanceId = data.instanceId,
            name = data.itemName,
            description = data.itemDescription,
            quality = data.qualityType,
            raceId = data.raceId,
            realmId = data.realmId,
            essenceId = data.essenceId,
            elementType = data.elementType,
            attackRange = data.attackRange,
            isCharacter = isCharacter,
            healthPoint = data.healthPoint,
            manaPoint = data.manaPoint,
            spiritPoint = data.spiritPoint,

            physicalDamagePoint = data.physicalDamagePoint,
            magicalDamagePoint = data.magicalDamagePoint,
            spiritDamagePoint = data.spiritDamagePoint,

            physicalDefensePoint = data.physicalDefensePoint,
            magicalDefensePoint = data.magicalDefensePoint,
            spiritDefensePoint = data.spiritDefensePoint,

            healthBonus = data.healthBonus.ToString(),
            manaBonus = data.manaBonus.ToString(),
            spiritBonus = data.spiritBonus.ToString(),

            skillIds = data.skillIds ?? new(),
            techniqueIds = data.techniqueIds ?? new(),
            equipmentIds = data.equipmentIds ?? new()

        };
    }
    public static ItemDataDto ToDto(ItemData data)
    {
        if (data == null) return null;

        var dto = new ItemDataDto
        {
            instanceId = data.instanceId,
            itemName = data.itemName,
            description = data.itemDescription,
            itemType = data.itemType,
            qualityType = data.qualityType,
            realmType = data.realmType,

            physicalDamage = data.physicalDamage + "",
            magicalDamage = data.magicalDamage + "",
            spiritDamage = data.spiritDamage + "",

            physicalDefense = data.physicalDefense + "",
            magicalDefense = data.magicalDefense + "",
            spiritDefense = data.spiritDefense + "",

            potentialPoints = data.potentialPoints
        };

        // 🔥 map riêng từng loại
        switch (data)
        {
            case EquipmentData e:
                dto.raceType = e.raceType;
                dto.equipmentType = e.equipmentType;
                break;

            case SkillData s:
                dto.raceType = s.raceType;
                dto.skillType = s.skillType;
                break;

            case TechniqueData t:
                dto.raceType = t.raceType;
                dto.techniqueType = t.techniqueType;
                break;
        }

        return dto;
    }
}