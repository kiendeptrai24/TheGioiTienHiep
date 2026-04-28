using System.Collections.Generic;
using Newtonsoft.Json;

public class ChampionResponseDto
{
    [JsonProperty("data")]
    public List<ChampionDataDto> Data;
}