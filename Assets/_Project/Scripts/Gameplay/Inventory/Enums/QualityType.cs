
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

[JsonConverter(typeof(StringEnumConverter))]
public enum QualityType
{
    [EnumMember(Value = "phàm")]
    Mortal = 0,   // Phàm
    [EnumMember(Value = "hoàng")]
    Yellow = 1,   // Hoàng
    [EnumMember(Value = "huyền")]
    Mystic = 2,   // Huyền
    [EnumMember(Value = "địa")]
    Earth = 3,    // Địa
    [EnumMember(Value = "thiên")]
    Heaven = 4    // Thiên
}
