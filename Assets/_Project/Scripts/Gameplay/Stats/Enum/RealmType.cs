
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

    // Kết Đan
    KetDan_SK,
    KetDan_TK,
    KetDan_HK,
    KetDan_DVM,

    // Nguyên Anh
    NguyenAnh_SK,
    NguyenAnh_TK,
    NguyenAnh_HK,
    NguyenAnh_DVM,

    // Hóa Thần
    HoaThan_SK,
    HoaThan_TK,
    HoaThan_HK,
    HoaThan_DVM,

    // Hợp Thể
    HopThe_SK,
    HopThe_TK,
    HopThe_HK,
    HopThe_DVM,

    // Độ Kiếp
    DoKiep_SK,
    DoKiep_TK,
    DoKiep_HK,
    DoKiep_DVM,

    // Đại Thừa
    DaiThua_SK,
    DaiThua_TK,
    DaiThua_HK,
    DaiThua_DVM,

    // Phi Thăng
    PhiThang
}
