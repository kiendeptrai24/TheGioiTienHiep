
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]

public enum ResourceSourceType
{
    [EnumMember(Value = "chung")]
    None,
    [EnumMember(Value = "khai thác")]
    KhaiThac,
    [EnumMember(Value = "quái vật")]
    QuaiVat
}