
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

[JsonConverter(typeof(StringEnumConverter))]
public enum SkillType
{
    [EnumMember(Value = "đơn trảm")]
    DonTram,            // Đơn Trảm
    [EnumMember(Value = "linh trảm")]
    LinhTram,           // Linh Trảm
    [EnumMember(Value = "liên kích chi thuật")]
    LienKichChiThuat,   // Liên Kích Chi Thuật
    [EnumMember(Value = "toàn lực nhất kích")]
    ToanLucNhatKich,    // Toàn Lực Nhất Kích
    [EnumMember(Value = "nhắm chuẩn")]
    NhamChuan,          // Nhắm Chuẩn
    [EnumMember(Value = "linh tiễn")]
    LinhTien,           // Linh Tiễn
    [EnumMember(Value = "vận linh tiễn")]
    VanLinhTien,        // Vận Lịnh Tiễn
    [EnumMember(Value = "vũ tiễn")]
    VuTien              // Vũ Tiễn
}
