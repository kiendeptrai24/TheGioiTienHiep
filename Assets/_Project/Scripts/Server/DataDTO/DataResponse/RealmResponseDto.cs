

using System.Collections.Generic;
using Newtonsoft.Json;

public class RealmResponseDto
{
    [JsonProperty("data")]
    public List<ItemRealmDataDto> Data { get; set; }
}