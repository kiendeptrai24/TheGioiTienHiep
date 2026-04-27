

using System.Collections.Generic;
using Newtonsoft.Json;

public class ItemResponseDto
{
    [JsonProperty("data")]
    public List<ItemDataDto> Data { get; set; }
}