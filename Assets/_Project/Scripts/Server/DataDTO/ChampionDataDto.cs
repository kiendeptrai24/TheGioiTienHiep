using System.Collections.Generic;
using Newtonsoft.Json;
public class ChampionDataDto
{
    [JsonProperty("Mã")]
    public string instanceId;

    [JsonProperty("Tên tướng")]
    public string name;

    [JsonProperty("Mô tả")]
    public string description;

    [JsonProperty("Nhân vật")]
    public bool? isCharacter;
    [JsonProperty("mã nhân vật")]
    public string characterId;
    [JsonProperty("vị trí X")]
    public int? posX;
    [JsonProperty("vị trí Y")]
    public int? posY;

    [JsonProperty("phẩm")]
    public QualityType quality;

    [JsonProperty("mã chủ tu")]
    public string essenceId;

    [JsonProperty("mã tộc")]
    public string raceId;

    [JsonProperty("Hệ")]
    public ElementType elementType;

    [JsonProperty("mã cảnh giới")]
    public string realmId;

    [JsonProperty("Tầm đánh")]
    public int attackRange;

    [JsonProperty("mã Công pháp")]
    public List<string> techniqueIds;

    [JsonProperty("mã kĩ năng")]
    public List<string> skillIds;
    [JsonProperty("mã trang bị")]
    public List<string> equipmentIds;

    [JsonProperty("Sinh lực điểm")]
    public int healthPoint;

    [JsonProperty("Linh lực điểm")]
    public int manaPoint;

    [JsonProperty("Linh thức điểm")]
    public int spiritPoint;

    [JsonProperty("Sát thương linh thể điểm")]
    public int physicalDamagePoint;

    [JsonProperty("Sát thương linh lực điểm")]
    public int magicalDamagePoint;

    [JsonProperty("Sát thương linh thức điểm")]
    public int spiritDamagePoint;

    [JsonProperty("Phòng ngự linh thể điểm")]
    public int physicalDefensePoint;

    [JsonProperty("Phòng ngự linh lực điểm")]
    public int magicalDefensePoint;

    [JsonProperty("Phòng ngự linh thức điểm")]
    public int spiritDefensePoint;

    [JsonProperty("Tăng sinh lực")]
    public string healthBonus;

    [JsonProperty("Tăng linh lực")]
    public string manaBonus;

    [JsonProperty("Tăng linh thức")]
    public string spiritBonus;
    [JsonProperty("Tăng Sát thương linh thể")]
    public string physicalDamageBonus;

    [JsonProperty("Tăng Sát thương linh lực")]
    public string magicalDamageBonus;
    [JsonProperty("Tăng Sát thương linh thức")]
    public string spiritDamageBonus;

    [JsonProperty("Tăng phòng ngự linh thể")]
    public string physicalDefenseBonus;

    [JsonProperty("Tăng phòng ngự linh lực")]
    public string magicalDefenseBonus;

    [JsonProperty("Tăng phòng ngự linh thức")]
    public string spiritDefenseBonus;
}