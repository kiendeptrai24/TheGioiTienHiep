

using System.Collections.Generic;
using Newtonsoft.Json;

public class ItemEssenceAndRaceResponseDto
{
    [JsonProperty("data")]
    public List<EssenceAndRaceDataDto> Data { get; set; }
}