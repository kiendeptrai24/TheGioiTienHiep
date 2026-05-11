using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[Serializable]
public class PlayerClientDataDto
{
    [JsonProperty("profile")]
    public PlayerProfileDTO profileRes;
    [JsonProperty("item data point")]
    public ItemDataPointDto itemDataPointRes;

    [JsonProperty("equipment in inventory")]
    public List<ItemDataDto> equipmentRes;
    [JsonProperty("item used")]
    public List<ItemDataDto> itemUsedRes;
    [JsonProperty("champion in inventory")]
    public List<ChampionDataDto> championInInventoryRes;

    [JsonProperty("champion in team")]
    public List<ChampionDataDto> championInTeamRes;
    public PlayerClientDataDto()
    {
        profileRes = new PlayerProfileDTO();
        itemDataPointRes = new ItemDataPointDto();
        equipmentRes = new List<ItemDataDto>();
        itemUsedRes = new List<ItemDataDto>();
        championInInventoryRes = new List<ChampionDataDto>();
        championInTeamRes = new List<ChampionDataDto>();
    }

}
