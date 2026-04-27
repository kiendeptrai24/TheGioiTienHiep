
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

[JsonConverter(typeof(StringEnumConverter))]
public enum TechniqueType
{
    [EnumMember(Value = "phàm nhân luyện linh quyết")]
    PhamNhanLuyenLinhQuyet, // Phàm Nhân Luyện Linh Quyết
    [EnumMember(Value = "linh vận quyết")]
    LinhVanQuyet,          // Linh Vận Quyết
    [EnumMember(Value = "yêu linh quyết")]
    YeuLinhQuyet,          // Yêu Linh Quyết
    [EnumMember(Value = "man ngưu bí pháp")]
    ManNguuBiPhap          // Man Ngưu Bí Pháp
}
