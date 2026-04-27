

using System.Collections.Generic;
using Newtonsoft.Json;

public class ItemRealmResponseDto
{
    [JsonProperty("data")]
    public List<ItemRealmDataDto> Data { get; set; }
}