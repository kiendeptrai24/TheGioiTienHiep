
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum EssenceType
{
    [EnumMember(Value = "chung")]
    General,
    [EnumMember(Value = "linh thể")]
    Physical,
    [EnumMember(Value = "linh lực")]
    Magical,
    [EnumMember(Value = "linh thức")]
    Spirit,
}