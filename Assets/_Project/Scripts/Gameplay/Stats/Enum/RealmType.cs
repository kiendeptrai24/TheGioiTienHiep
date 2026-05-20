
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

[JsonConverter(typeof(StringEnumConverter))]
public enum RealmType
{
    // Luyện Khí

    [EnumMember(Value = "luyện khí 1")]
    LuyenKhi_1,
    [EnumMember(Value = "luyện khí 2")]
    LuyenKhi_2,
    [EnumMember(Value = "luyện khí 3")]
    LuyenKhi_3,
    [EnumMember(Value = "luyện khí 4")]
    LuyenKhi_4,
    [EnumMember(Value = "luyện khí 5")]
    LuyenKhi_5,
    [EnumMember(Value = "luyện khí 6")]
    LuyenKhi_6,
    [EnumMember(Value = "luyện khí 7")]
    LuyenKhi_7,
    [EnumMember(Value = "luyện khí 8")]
    LuyenKhi_8,
    [EnumMember(Value = "luyện khí 9")]
    LuyenKhi_9,

    // Trúc Cơ
    [EnumMember(Value = "trúc cơ sơ kỳ")]
    TrucCo_SK,
    [EnumMember(Value = "trúc cơ trung kỳ")]
    TrucCo_TK,
    [EnumMember(Value = "trúc cơ hậu kỳ")]
    TrucCo_HK,
    [EnumMember(Value = "trúc cơ đại viện mãn")]
    TrucCo_DVM,
    [EnumMember(Value = "kết đan sơ kỳ")]
    // Kết Đan
    KetDan_SK,
    [EnumMember(Value = "kết đan trung kỳ")]
    KetDan_TK,
    [EnumMember(Value = "kết đan hậu kỳ")]
    KetDan_HK,
    [EnumMember(Value = "kết đan đại viên mãn")]
    KetDan_DVM,

    // Nguyên Anh
    [EnumMember(Value = "nguyên anh sơ kỳ")]
    NguyenAnh_SK,
    [EnumMember(Value = "nguyên anh trung kỳ")]
    NguyenAnh_TK,
    [EnumMember(Value = "nguyên anh hậu kỳ")]
    NguyenAnh_HK,
    [EnumMember(Value = "nguyên anh đại viên mãn")]
    NguyenAnh_DVM,

    // Hóa Thần
    [EnumMember(Value = "hóa thần sơ kỳ")]
    HoaThan_SK,
    [EnumMember(Value = "hóa thần trung kỳ")]
    HoaThan_TK,
    [EnumMember(Value = "hóa thần hậu kỳ")]
    HoaThan_HK,
    [EnumMember(Value = "hóa thần đại viên mãn")]
    HoaThan_DVM,

    // Hợp Thể
    [EnumMember(Value = "hợp thể sơ kỳ")]
    HopThe_SK,
    [EnumMember(Value = "hợp thể trung kỳ")]
    HopThe_TK,
    [EnumMember(Value = "hợp thể hậu kỳ")]
    HopThe_HK,
    [EnumMember(Value = "hợp thể đại viên mãn")]
    HopThe_DVM,

    // Độ Kiếp
    [EnumMember(Value = "độ kiếp sơ kỳ")]
    DoKiep_SK,
    [EnumMember(Value = "độ kiếp trung kỳ")]
    DoKiep_TK,
    [EnumMember(Value = "độ kiếp hậu kỳ")]
    DoKiep_HK,
    [EnumMember(Value = "độ kiếp đại viên mãn")]
    DoKiep_DVM,

    // Đại Thừa
    [EnumMember(Value = "đại thừa sơ kỳ")]
    DaiThua_SK,
    [EnumMember(Value = "đại thừa trung kỳ")]
    DaiThua_TK,
    [EnumMember(Value = "đại thừa hậu kỳ")]
    DaiThua_HK,
    [EnumMember(Value = "đại thừa đại viên mãn")]
    DaiThua_DVM,

    // Phi Thăng
    [EnumMember(Value = "phi thăng")]
    PhiThang
}
