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
    [JsonProperty("Pill")]
    public List<PillDataDto> pillRes;
    [JsonProperty("Shop")]
    public List<ShopDataDto> shopRes;
    [JsonProperty("SpiritStoneMine")]
    public List<SpiritStoneMineDataDto> spiritStoneMineRes;
    [JsonProperty("DemonBeast")]
    public List<DemonBeastDataDto> demonBeastRes;
}