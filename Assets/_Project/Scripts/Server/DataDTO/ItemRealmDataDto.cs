using Newtonsoft.Json;

public class ItemRealmDataDto
{
    [JsonProperty("Mã")]
    public string instanceId;

    [JsonProperty("Cảnh giới")]
    public RealmType realmType;

    [JsonProperty("Sinh lực")]
    public int health;

    [JsonProperty("Linh lực")]
    public int mana;

    [JsonProperty("Linh thức")]
    public int spirit;

    [JsonProperty("Sát thương linh thể")]
    public int physicalDamage;

    [JsonProperty("Sát thương linh lực")]
    public int magicalDamage;

    [JsonProperty("Sát thương linh thức")]
    public int spiritDamage;

    [JsonProperty("Phòng ngự linh thể")]
    public int physicalDefense;

    [JsonProperty("Phòng ngự linh lực")]
    public int magicalDefense;

    [JsonProperty("Phòng ngự linh thức")]
    public int spiritDefense;

    [JsonProperty("Pv linh thức")]
    public int spiritCritRate;

    [JsonProperty("Tddc")]
    public int movementSpeed;

    [JsonProperty("Tiềm năng điểm")]
    public int potentialPoints;

    [JsonProperty("Kĩ năng điểm")]
    public int skillPoints;

    [JsonProperty("Linh thạch")]
    public int lThach;

    [JsonProperty("Vật phẩm")]
    public string? item;

    [JsonProperty("Tỉ lệ")]
    public string rate;

    [JsonProperty("Tăng tỉ lệ")]
    public string increaseRate;

    [JsonProperty("Time")]
    public string time;
}