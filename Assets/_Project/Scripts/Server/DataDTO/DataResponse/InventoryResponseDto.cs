

using System.Collections.Generic;
using Newtonsoft.Json;

public class EquipmentResponseDto
{
    [JsonProperty("data")]
    public List<ItemDataDto> Data { get; set; }
}