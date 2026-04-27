using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
[JsonConverter(typeof(StringEnumConverter))]
public enum RaceType
{
    [EnumMember(Value = "chung")]
    General,      // Chung - Tộc
    [EnumMember(Value = "nhân")]
    Human,       // Nhân
    [EnumMember(Value = "yêu")]
    Beast,       // Yêu
    [EnumMember(Value = "thiên")]
    Celestial,   // Thiên
    [EnumMember(Value = "ma")]
    Demon,       // Ma
}
