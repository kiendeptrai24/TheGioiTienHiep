using Newtonsoft.Json;

public class ItemDataDto
{
    [JsonProperty("mã")]
    public string itemInstanceId;

    [JsonProperty("tên")]
    public string itemName;

    [JsonProperty("mô tả")]
    public string description;

    [JsonProperty("loại")]
    public ItemType itemType;

    [JsonProperty("cảnh giới")]
    public RealmType realmType;

    [JsonProperty("phẩm")]
    public QualityType qualityType;

    [JsonProperty("tộc")]
    public RaceType raceType;

    [JsonProperty("sinh lực")]
    public float health;

    [JsonProperty("linh lực")]
    public float mana;

    [JsonProperty("linh thức")]
    public int spirit;

    [JsonProperty("sát thương sing lực")]
    public float physicalDamage;

    [JsonProperty("sát thương linh lực")]
    public float magicalDamage;

    [JsonProperty("sát thương linh thức")]
    public float spiritDamage;

    [JsonProperty("phòng ngự sing lực")]
    public float physicalDefense;

    [JsonProperty("phòng ngự linh lực")]
    public float magicalDefense;

    [JsonProperty("phòng ngự linh thức")]
    public float sppiritDefense;

    [JsonProperty("điểm tiềm năng")]
    public int potentialPoints;

    [JsonProperty("loại trang bị")]
    public EquipmentType? equipmentType;

    [JsonProperty("loại công pháp")]
    public TechniqueType? techniqueType;

    [JsonProperty("loại kĩ năng")]
    public SkillType? skillType;
}