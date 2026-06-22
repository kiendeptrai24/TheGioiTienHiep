using Newtonsoft.Json;

public class ItemDataDto
{
    [JsonProperty("mã")]
    public string instanceId;
    [JsonProperty("mã gốc")]
    public string itemBaseId;

    [JsonProperty("tên")]
    public string itemName;

    [JsonProperty("mô tả")]
    public string description;
    [JsonProperty("Hình")]
    public string iconPath;

    [JsonProperty("loại vật phẩm")]
    public ItemType itemType;
    [JsonProperty("cộng dồn")]
    public bool canStack;
    [JsonProperty("số lượng")]
    public int currentStack;

    [JsonProperty("tộc")]
    public RaceType raceType;

    [JsonProperty("phẩm")]
    public QuanlityType qualityType;

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
    [JsonProperty("thời gian hồi chiêu")]
    public float cooldown;
    [JsonProperty("thời gian tung chiêu")]
    public float animationDuration;
    [JsonProperty("thời gian chiêu thức")]
    public float castTime;
    [JsonProperty("tỉ lệ")]
    public string rate;
    [JsonProperty("loại thuốc")]
    public PillType pillType;
    [JsonProperty("Khí huyết tiêu hao")]
    public int healthCost;
    [JsonProperty("Linh lực tiêu hao")]
    public int manaCost;
    [JsonProperty("Linh thức tiêu hao")]
    public int spiritCost;
}