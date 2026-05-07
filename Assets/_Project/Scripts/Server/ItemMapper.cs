public static class ItemMapper
{
    public static ItemData ToData(ItemDataDto dto)
    {
        return new ItemData
        {
            instanceId = dto.instanceId,
            itemName = dto.itemName,
            itemDescription = dto.description,
            physicalDamage = DataParseUtils.ParseNumberOrPercent(dto.physicalDamage),
            magicalDamage = DataParseUtils.ParseNumberOrPercent(dto.magicalDamage),
            spiritDamage = DataParseUtils.ParseNumberOrPercent(dto.spiritDamage),
            physicalDefense = DataParseUtils.ParseNumberOrPercent(dto.physicalDefense),
            magicalDefense = DataParseUtils.ParseNumberOrPercent(dto.magicalDefense),
            spiritDefense = DataParseUtils.ParseNumberOrPercent(dto.spiritDefense),
            health = DataParseUtils.ParseNumberOrPercent(dto.health),
            mana = DataParseUtils.ParseNumberOrPercent(dto.mana),
            spirit = DataParseUtils.ParseNumberOrPercent(dto.spirit),
            potentialPoints = dto.potentialPoints,
            realmType = dto.realmType,
        };
    }

    public static ItemDataDto ToDto(ItemData data)
    {
        return new ItemDataDto
        {
            instanceId = data.instanceId,
            itemName = data.itemName,
            description = data.itemDescription,
            physicalDamage = data.physicalDamage.ToString(),
            magicalDamage = data.magicalDamage.ToString(),
            spiritDamage = data.spiritDamage.ToString(),
            physicalDefense = data.physicalDefense.ToString(),
            magicalDefense = data.magicalDefense.ToString(),
            spiritDefense = data.spiritDefense.ToString(),
            potentialPoints = data.potentialPoints,
            realmType = data.realmType,
        };
    }
}