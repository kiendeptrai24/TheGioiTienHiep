using System;
using System.Collections.Generic;

public static class EnumTranslator
{
    private static readonly Dictionary<Enum, string> _translations = new Dictionary<Enum, string>
    {
        // CultivationStage
        { RealmType.LuyenKhi_1, "Luyện Khí Kỳ - tần 1" },
        { RealmType.LuyenKhi_2, "Luyện Khí Kỳ - tần 2" },
        { RealmType.LuyenKhi_3, "Luyện Khí Kỳ - tần 3" },
        { RealmType.LuyenKhi_4, "Luyện Khí Kỳ - tần 4" },
        { RealmType.LuyenKhi_5, "Luyện Khí Kỳ - tần 5" },
        { RealmType.LuyenKhi_6, "Luyện Khí Kỳ - tần 6" },
        { RealmType.LuyenKhi_7, "Luyện Khí Kỳ - tần 7" },
        { RealmType.LuyenKhi_8, "Luyện Khí Kỳ - tần 8" },
        { RealmType.LuyenKhi_9, "Luyện Khí Kỳ - tần 9" },
        { RealmType.TrucCo_SK, "Trúc Cơ Sơ Kỳ" },
        { RealmType.TrucCo_TK, "Trúc Cơ Trung Kỳ" },
        { RealmType.TrucCo_HK, "Trúc Cơ Hậu Kỳ" },
        { RealmType.TrucCo_DVM, "Trúc Cơ Đỉnh Viên Mãn" },
        { RealmType.KetDan_SK, "Kết Đan Sơ Kỳ" },
        { RealmType.KetDan_TK, "Kết Đan Trung Kỳ" },
        { RealmType.KetDan_HK, "Kết Đan Hậu Kỳ" },
        { RealmType.KetDan_DVM, "Kết Đan Đỉnh Viên Mãn" },
        { RealmType.NguyenAnh_SK, "Nguyên Anh Sơ Kỳ" },
        { RealmType.NguyenAnh_TK, "Nguyên Anh Trung Kỳ" },
        { RealmType.NguyenAnh_HK, "Nguyên Anh Hậu Kỳ" },
        { RealmType.NguyenAnh_DVM, "Nguyên Anh Đỉnh Viên Mãn" },
        { RealmType.HoaThan_SK, "Hóa Thần Sơ Kỳ" },
        { RealmType.HoaThan_TK, "Hóa Thần Trung Kỳ" },
        { RealmType.HoaThan_HK, "Hóa Thần Hậu Kỳ" },
        { RealmType.HoaThan_DVM, "Hóa Thần Đỉnh Viên Mãn" },
        { RealmType.HopThe_SK, "Hợp Thể Sơ Kỳ" },
        { RealmType.HopThe_TK, "Hợp Thể Trung Kỳ" },
        { RealmType.HopThe_HK, "Hợp Thể Hậu Kỳ" },
        { RealmType.HopThe_DVM, "Hợp Thể Đỉnh Viên Mãn" },
        { RealmType.DoKiep_SK, "Độ Kiếp Sơ Kỳ" },
        { RealmType.DoKiep_TK, "Độ Kiếp Trung Kỳ" },
        { RealmType.DoKiep_HK, "Độ Kiếp Hậu Kỳ" },
        { RealmType.DoKiep_DVM, "Độ Kiếp Đỉnh Viên Mãn" },
        { RealmType.DaiThua_SK, "Đại Thừa Sơ Kỳ" },
        { RealmType.DaiThua_TK, "Đại Thừa Trung Kỳ" },
        { RealmType.DaiThua_HK, "Đại Thừa Hậu Kỳ" },
        { RealmType.DaiThua_DVM, "Đại Thừa Đỉnh Viên Mãn" },
        { RealmType.PhiThang, "Phi Thăng" },

        // EssenceType
        { EssenceType.General, "Chung" },
        { EssenceType.Physical, "Linh Thể" },
        { EssenceType.Magical, "Linh Lực" },
        { EssenceType.Spirit, "Linh Thức" },

        // RaceType
        { RaceType.General, "Chung" },
        { RaceType.Human, "Nhân" },
        { RaceType.Beast, "Yêu" },
        { RaceType.Celestial, "Thiên" },
        { RaceType.Demon, "Ma" },

        // ElementType
        { ElementType.Neutral, "Chung" },
        { ElementType.Metal, "Kim" },
        { ElementType.Wood, "Mộc" },
        { ElementType.Water, "Thủy" },
        { ElementType.Fire, "Hỏa" },
        { ElementType.Earth, "Thổ" },
        { ElementType.Wind, "Phong" },
        { ElementType.Lightning, "Lôi" },
        { ElementType.Yin, "Âm" },
        { ElementType.Yang, "Dương" },
        { ElementType.Soul, "Hồn" },
        { ElementType.Divine, "Thần" },

        // EquipmentType
        { EquipmentType.None, "Không có trang bị" },
        { EquipmentType.Helmet, "Nón" },
        { EquipmentType.Weapon, "Vũ Khí" },
        { EquipmentType.Armor, "Áo" },
        { EquipmentType.Necklace, "Dây Chuyền" },
        { EquipmentType.Pants, "Quần" },
        { EquipmentType.Ring, "Nhẫn" },
        { EquipmentType.Belt, "Thắt Lưng" },
        { EquipmentType.Gloves, "Bao Tay" },
        { EquipmentType.Boots, "Giày" },
        { EquipmentType.Pet, "Thú cưng" },

        // ItemType
        { ItemType.Material, "Nguyên liệu" },
        { ItemType.Equipment, "Trang bị" },
        { ItemType.Technique, "Công pháp" },
        { ItemType.Skill, "Kỹ năng" },
        { ItemType.Other, "Khác" },

        // QualityType
        { QualityType.Mortal, "Phàm" },
        { QualityType.Yellow, "Hoàng" },
        { QualityType.Mystic, "Huyền" },
        { QualityType.Earth, "Địa" },
        { QualityType.Heaven, "Thiên" },

        // TechniqueType
        { TechniqueType.PhamNhanLuyenLinhQuyet, "Phàm Nhân Luyện Linh Quyết" },
        { TechniqueType.LinhVanQuyet, "Linh Vận Quyết" },
        { TechniqueType.YeuLinhQuyet, "Yêu Linh Quyết" },
        { TechniqueType.ManNguuBiPhap, "Man Ngưu Bí Pháp" },

        // SkillType
        { SkillType.DonTram, "Đơn Trảm" },
        { SkillType.LinhTram, "Linh Trảm" },
        { SkillType.LienKichChiThuat, "Liên Kích Chi Thuật" },
        { SkillType.ToanLucNhatKich, "Toàn Lực Nhất Kích" },
        { SkillType.NhamChuan, "Nhắm Chuẩn" },
        { SkillType.LinhTien, "Linh Tiễn" },
        { SkillType.VanLinhTien, "Vận Lịnh Tiễn" },
        { SkillType.VuTien, "Vũ Tiễn" },
    };

    public static string ToVietnamese(Enum value)
    {
        if (_translations.TryGetValue(value, out var result))
            return result;
        return value.ToString();
    }
}
