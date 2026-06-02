using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

[JsonConverter(typeof(StringEnumConverter))]
public enum ItemType
{
    [EnumMember(Value = "vật phẩm")]
    Material = 0,

    [EnumMember(Value = "trang bị")]
    Equipment,

    [EnumMember(Value = "công pháp")]
    Technique,

    [EnumMember(Value = "kĩ năng")]
    Skill,

    [EnumMember(Value = "tướng")]
    Champion,

    [EnumMember(Value = "thuốc")]
    Pill,
    [EnumMember(Value = "khác")]
    Other,
}