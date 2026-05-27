using Newtonsoft.Json;
public class CharacterDataDto
{
    [JsonProperty("Mã")]
    public string instanceId;
    [JsonProperty("Mô tả")]
    public string description;
    [JsonProperty("Hình")]
    public string iconPath;
    [JsonProperty("mã tộc")]
    public string raceId;
    [JsonProperty("mã cảnh giới")]
    public string realmId;
    [JsonProperty("mã chủ tu")]
    public string? essenceId;

}