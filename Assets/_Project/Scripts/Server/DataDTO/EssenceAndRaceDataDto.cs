using Newtonsoft.Json;

public class EssenceAndRaceDataDto
{
    [JsonProperty("Mã")]
    public string instanceId;

    [JsonProperty("Loại")]
    public EssenceAndRaceType type;

    [JsonProperty("Sinh lực điểm")]
    public string healthPoint;

    [JsonProperty("Linh lực điểm")]
    public string manaPoint;

    [JsonProperty("Linh thức điểm")]
    public string spiritPoint;

    [JsonProperty("Sát thương linh thể điểm")]
    public string physicalDamagePoint;

    [JsonProperty("Sát thương linh lực điểm")]
    public string magicalDamagePoint;

    [JsonProperty("Sát thương linh thức điểm")]
    public string spiritDamagePoint;

    [JsonProperty("Phòng ngự linh thể điểm")]
    public string physicalDefensePoint;

    [JsonProperty("Phòng ngự linh lực điểm")]
    public string magicalDefensePoint;

    [JsonProperty("Phòng ngự linh thức điểm")]
    public string spiritDefensePoint;

    [JsonProperty("Phạm vi linh thức điểm")]
    public string spiritRangePoint;

    [JsonProperty("Tddc điểm")]
    public string movementSpeedPoint;

    [JsonProperty("Loại chủ tu")]
    public EssenceType? essenceType;

    [JsonProperty("Loại tộc")]
    public RaceType? raceType;
}