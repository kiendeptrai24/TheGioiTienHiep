

using System.Collections.Generic;
using Newtonsoft.Json;

public class InventoryResponseDto
{
    [JsonProperty("data")]
    public List<ItemDataDto> Data { get; set; }
}