
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum EssenceAndRaceType
{
    [EnumMember(Value = "chủ tu")]
    Essence,
    [EnumMember(Value = "tộc")]
    Race
}