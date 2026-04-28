

using System.Collections.Generic;
using Newtonsoft.Json;

public class ShopResponseDto
{
    [JsonProperty("data")]
    public List<ShopDataDto> Data { get; set; }
}