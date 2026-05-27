

using Newtonsoft.Json;

public class SpiritStoneMineDataDto
{
    [JsonProperty("Mã")]
    public string instanceId;
    [JsonProperty("Loại")]
    public ResourceSourceType resourceSourceType;
    [JsonProperty("Tên")]
    public string name;
    [JsonProperty("Hình")]
    public string iconPath;
    [JsonProperty("Cấp")]
    public int level;
    [JsonProperty("Trữ Lượng")]
    public int amount;
    [JsonProperty("Tốc Độ Khai Thác Số/s")]
    public int yieldPerHarvest;
}