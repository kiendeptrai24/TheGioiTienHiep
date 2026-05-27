

using Newtonsoft.Json;

public class DemonBeastDataDto
{
    [JsonProperty("Mã")]
    public string instanceId;
    [JsonProperty("Tên")]
    public string name;
    [JsonProperty("Hình")]
    public string iconPath;
    [JsonProperty("Loại")]
    public ResourceSourceType resourceSourceType;
    [JsonProperty("Mô Tả")]
    public string description;
    [JsonProperty("Cấp")]
    public int level;
    [JsonProperty("Linh Thạch")]
    public ulong lthach;
}