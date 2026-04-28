using Newtonsoft.Json;
public class CharacterDataDto
{
    [JsonProperty("Mã")]
    public string instanceId;
    [JsonProperty("Mô tả")]
    public string description;
    [JsonProperty("Loại tộc")]
    public RaceType raceType;
    [JsonProperty("Cảnh giới")]
    public RealmType realmType;
    
}