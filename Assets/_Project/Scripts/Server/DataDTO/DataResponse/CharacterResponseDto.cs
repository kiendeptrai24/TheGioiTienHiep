

using System.Collections.Generic;
using Newtonsoft.Json;

public class CharacterResponseDto
{
    [JsonProperty("data")]
    public List<CharacterDataDto> Data { get; set; }
}