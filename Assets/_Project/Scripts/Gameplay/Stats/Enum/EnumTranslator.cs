using System;
using System.Collections.Generic;

public static class EnumTranslator
{
    private static readonly Dictionary<Enum, string> _translations = new Dictionary<Enum, string>
    {
        // CultivationStage
        { CultivationStage.LuyenKhi_1, "Luyện Khí Kỳ - tần 1" },
        { CultivationStage.LuyenKhi_2, "Luyện Khí Kỳ - tần 2" },
        { CultivationStage.LuyenKhi_3, "Luyện Khí Kỳ - tần 3" },
        { CultivationStage.LuyenKhi_4, "Luyện Khí Kỳ - tần 4" },
        { CultivationStage.LuyenKhi_5, "Luyện Khí Kỳ - tần 5" },
        { CultivationStage.LuyenKhi_6, "Luyện Khí Kỳ - tần 6" },
        { CultivationStage.LuyenKhi_7, "Luyện Khí Kỳ - tần 7" },
        { CultivationStage.LuyenKhi_8, "Luyện Khí Kỳ - tần 8" },
        { CultivationStage.LuyenKhi_9, "Luyện Khí Kỳ - tần 9" },
        { CultivationStage.TrucCo_SK, "Trúc Cơ Sơ Kỳ" },
        { CultivationStage.TrucCo_TK, "Trúc Cơ Trung Kỳ" },
        { CultivationStage.TrucCo_HK, "Trúc Cơ Hậu Kỳ" },
        { CultivationStage.TrucCo_DVM, "Trúc Cơ Đỉnh Viên Mãn" },
        { CultivationStage.KetDan_SK, "Kết Đan Sơ Kỳ" },
        { CultivationStage.KetDan_TK, "Kết Đan Trung Kỳ" },
        { CultivationStage.KetDan_HK, "Kết Đan Hậu Kỳ" },
        { CultivationStage.KetDan_DVM, "Kết Đan Đỉnh Viên Mãn" },
        { CultivationStage.NguyenAnh_SK, "Nguyên Anh Sơ Kỳ" },
        { CultivationStage.NguyenAnh_TK, "Nguyên Anh Trung Kỳ" },
        { CultivationStage.NguyenAnh_HK, "Nguyên Anh Hậu Kỳ" },
        { CultivationStage.NguyenAnh_DVM, "Nguyên Anh Đỉnh Viên Mãn" },
        { CultivationStage.HoaThan_SK, "Hóa Thần Sơ Kỳ" },
        { CultivationStage.HoaThan_TK, "Hóa Thần Trung Kỳ" },
        { CultivationStage.HoaThan_HK, "Hóa Thần Hậu Kỳ" },
        { CultivationStage.HoaThan_DVM, "Hóa Thần Đỉnh Viên Mãn" },
        { CultivationStage.HopThe_SK, "Hợp Thể Sơ Kỳ" },
        { CultivationStage.HopThe_TK, "Hợp Thể Trung Kỳ" },
        { CultivationStage.HopThe_HK, "Hợp Thể Hậu Kỳ" },
        { CultivationStage.HopThe_DVM, "Hợp Thể Đỉnh Viên Mãn" },
        { CultivationStage.DoKiep_SK, "Độ Kiếp Sơ Kỳ" },
        { CultivationStage.DoKiep_TK, "Độ Kiếp Trung Kỳ" },
        { CultivationStage.DoKiep_HK, "Độ Kiếp Hậu Kỳ" },
        { CultivationStage.DoKiep_DVM, "Độ Kiếp Đỉnh Viên Mãn" },
        { CultivationStage.DaiThua_SK, "Đại Thừa Sơ Kỳ" },
        { CultivationStage.DaiThua_TK, "Đại Thừa Trung Kỳ" },
        { CultivationStage.DaiThua_HK, "Đại Thừa Hậu Kỳ" },
        { CultivationStage.DaiThua_DVM, "Đại Thừa Đỉnh Viên Mãn" },
        { CultivationStage.PhiThang, "Phi Thăng" },

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
