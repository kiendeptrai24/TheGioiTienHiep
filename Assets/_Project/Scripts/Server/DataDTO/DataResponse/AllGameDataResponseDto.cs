using System.Collections.Generic;
using Newtonsoft.Json;

public class AllGameDataResponseDto
{
    [JsonProperty("Champion")]
    public List<ChampionDataDto> championRes;

    [JsonProperty("Character")]
    public List<CharacterDataDto> characterRes;
    [JsonProperty("EssenceAndRace")]
    public List<EssenceAndRaceDataDto> essenceAndRaceRes;
    [JsonProperty("Realm")]
    public List<ItemRealmDataDto> realmRes;
    [JsonProperty("Equipment")]
    public List<ItemDataDto> equipmentRes;
    [JsonProperty("Shop")]
    public List<ShopDataDto> shopRes;
}