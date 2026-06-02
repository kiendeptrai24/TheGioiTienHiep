

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

// Enum định nghĩa các loại trang bị trong game (theo thứ tự hiển thị UI)
[JsonConverter(typeof(StringEnumConverter))]
public enum EquipmentType
{
    [EnumMember(Value = "không có trang bị")]
    None = 0,          // Không có trang bị
    [EnumMember(Value = "nón")]
    Helmet,            // Nón
    [EnumMember(Value = "vũ khí")]
    Weapon,            // Vũ Khí
    [EnumMember(Value = "áo")]
    Armor,             // Áo
    [EnumMember(Value = "dây chuyền")]
    Necklace,          // Dây Chuyền
    [EnumMember(Value = "quần")]
    Pants,             // Quần
    [EnumMember(Value = "nhẫn")]
    Ring,              // Nhẫn
    [EnumMember(Value = "đai lưng")]
    Belt,              // Đai Lưng
    [EnumMember(Value = "bao tay")]
    Gloves,            // Bao Tay
    [EnumMember(Value = "giày")]
    Boots,             // Giày
    [EnumMember(Value = "thú cưng")]
    Pet,               // Thú cưng (nếu có dùng)
}
