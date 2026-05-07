

using Newtonsoft.Json;

public class SpiritStoneMineDataDto
{
    [JsonProperty("Mã")]
    public string instanceId;
    [JsonProperty("Tên")]
    public string name;
    [JsonProperty("Cấp")]
    public int level;
    [JsonProperty("Trữ Lượng")]
    public int amount;
    [JsonProperty("Tốc Độ Khai Thác Số/s")]
    public int yieldPerHarvest;
}