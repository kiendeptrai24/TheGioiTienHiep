using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum PillType
{
    [EnumMember(Value = "Đột phá")]
    Breakthrough, // Đột phá
    [EnumMember(Value = "Tu luyện")]
    Cultivation, // Tu luyện
    [EnumMember(Value = "Hồi phục")]
    Recovery, // Hồi phục
    [EnumMember(Value = "Tăng cường")]
    Buff, // Tăng cường
    [EnumMember(Value = "Đặc biệt")]
    Special // Đặc biệt
}