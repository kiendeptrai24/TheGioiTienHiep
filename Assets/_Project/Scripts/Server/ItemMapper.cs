public static class ItemMapper
{
    public static ItemData ToData(ItemDataDto dto)
    {
        return new ItemData
        {
            instanceId = dto.instanceId,
            itemName = dto.itemName,
            itemDescription = dto.description,
            physicalDamage = dto.physicalDamage,
            magicalDamage = dto.magicalDamage,
            spiritDamage = dto.spiritDamage,
            physicalDefense = dto.physicalDefense,
            magicalDefense = dto.magicalDefense,
            spiritDefense = dto.spiritDefense,
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
            physicalDamage = data.physicalDamage,
            magicalDamage = data.magicalDamage,
            spiritDamage = data.spiritDamage,
            physicalDefense = data.physicalDefense,
            magicalDefense = data.magicalDefense,
            spiritDefense = data.spiritDefense,
            potentialPoints = data.potentialPoints,
            realmType = data.realmType,
        };
    }
}