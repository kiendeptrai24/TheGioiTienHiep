

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

[JsonConverter(typeof(StringEnumConverter))]
public enum ElementType
{
    [EnumMember(Value = "chung")]
    Neutral,   // Chung
    [EnumMember(Value = "kim")]
    Metal,      // Kim
    [EnumMember(Value = "mộc")]
    Wood,       // Mộc
    [EnumMember(Value = "thủy")]
    Water,      // Thủy
    [EnumMember(Value = "hỏa")]
    Fire,       // Hỏa
    [EnumMember(Value = "thổ")]
    Earth,      // Thổ
    [EnumMember(Value = "phong")]
    Wind,       // Phong
    [EnumMember(Value = "lôi")]
    Lightning,  // Lôi
    [EnumMember(Value = "âm")]
    Yin,        // Âm
    [EnumMember(Value = "dương")]
    Yang,       // Dương
    [EnumMember(Value = "hồn")]
    Soul,       // Hồn
    [EnumMember(Value = "thần")]
    Divine      // Thần
}
