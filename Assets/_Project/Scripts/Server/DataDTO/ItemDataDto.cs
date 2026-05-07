using Newtonsoft.Json;

public class ItemDataDto
{
    [JsonProperty("mã")]
    public string instanceId;

    [JsonProperty("tên")]
    public string itemName;

    [JsonProperty("mô tả")]
    public string description;

    [JsonProperty("loại")]
    public ItemType itemType;

    [JsonProperty("tộc")]
    public RaceType raceType;

    [JsonProperty("phẩm")]
    public QualityType qualityType;

    [JsonProperty("cảnh giới")]
    public RealmType realmType;

    [JsonProperty("sinh lực")]
    public string health;

    [JsonProperty("linh lực")]
    public string mana;
    [JsonProperty("linh thức")]
    public string spirit;

    [JsonProperty("sát thương sing lực")]
    public string physicalDamage;

    [JsonProperty("sát thương linh lực")]
    public string magicalDamage;

    [JsonProperty("sát thương linh thức")]
    public string spiritDamage;

    [JsonProperty("phòng ngự sing lực")]
    public string physicalDefense;

    [JsonProperty("phòng ngự linh lực")]
    public string magicalDefense;

    [JsonProperty("phòng ngự linh thức")]
    public string spiritDefense;

    [JsonProperty("điểm tiềm năng")]
    public int potentialPoints;

    [JsonProperty("loại trang bị")]
    public EquipmentType? equipmentType;

    [JsonProperty("loại công pháp")]
    public TechniqueType? techniqueType;

    [JsonProperty("loại kĩ năng")]
    public SkillType? skillType;
}