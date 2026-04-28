using System.Collections.Generic;
using Newtonsoft.Json;
public class ShopDataDto
{
    [JsonProperty("Mã")]
    public string instanceId;
    [JsonProperty("Giá linh thạch")]
    public ulong price;
}