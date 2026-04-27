using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

[JsonConverter(typeof(StringEnumConverter))]
public enum ItemType
{
    [EnumMember(Value = "thường")]
    Material = 0,

    [EnumMember(Value = "trang bị")]
    Equipment,

    [EnumMember(Value = "công pháp")]
    Technique,

    [EnumMember(Value = "kĩ năng")]
    Skill,

    [EnumMember(Value = "khác")]
    Other,

    [EnumMember(Value = "tướng")]
    Champion,

    [EnumMember(Value = "điều kiện")]
    Condition
}