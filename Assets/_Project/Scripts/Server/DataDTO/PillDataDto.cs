

using Newtonsoft.Json;

public class PillDataDto
{
    [JsonProperty("Mã")]
    public string instanceId;
    [JsonProperty("Tên")]
    public string itemName;
    [JsonProperty("Mô Tả")]
    public string itemDescription;
    [JsonProperty("Cảnh Giới")]
    public RealmType realmType;
    [JsonProperty("Loại")]
    public PillType pillType;
    [JsonProperty("Phẩm")]
    public QuanlityType quanlity;
    [JsonProperty("Hệ")]
    public ElementType elementType;

    [JsonProperty("Sinh lực")]
    public string health;

    [JsonProperty("Linh lực")]
    public string mana;

    [JsonProperty("Linh thức")]
    public string spirit;
    [JsonProperty("Tỷ lệ")]
    public string rate;
}