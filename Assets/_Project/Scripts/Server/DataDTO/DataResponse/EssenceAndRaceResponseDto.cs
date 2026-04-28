

using System.Collections.Generic;
using Newtonsoft.Json;

public class EssenceAndRaceResponseDto
{
    [JsonProperty("data")]
    public List<EssenceAndRaceDataDto> Data { get; set; }
}