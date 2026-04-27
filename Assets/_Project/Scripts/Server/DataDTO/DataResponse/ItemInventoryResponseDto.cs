

using System.Collections.Generic;
using Newtonsoft.Json;

public class ItemInventoryResponseDto
{
    [JsonProperty("data")]
    public List<ItemDataDto> Data { get; set; }
}