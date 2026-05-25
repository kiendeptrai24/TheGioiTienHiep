using System;
using System.Collections.Generic;

public static class EnumTranslator
{

    private static readonly Dictionary<Enum, string> _translations = new Dictionary<Enum, string>
    {
        // CultivationStageg
        { RealmType.LuyenKhi_1, "Luyện Khí Kỳ - tầng 1" },
        { RealmType.LuyenKhi_2, "Luyện Khí Kỳ - tầng 2" },
        { RealmType.LuyenKhi_3, "Luyện Khí Kỳ - tầng 3" },
        { RealmType.LuyenKhi_4, "Luyện Khí Kỳ - tầng 4" },
        { RealmType.LuyenKhi_5, "Luyện Khí Kỳ - tầng 5" },
        { RealmType.LuyenKhi_6, "Luyện Khí Kỳ - tầng 6" },
        { RealmType.LuyenKhi_7, "Luyện Khí Kỳ - tầng 7" },
        { RealmType.LuyenKhi_8, "Luyện Khí Kỳ - tầng 8" },
        { RealmType.LuyenKhi_9, "Luyện Khí Kỳ - tầng 9" },
        { RealmType.TrucCo_SK, "Trúc Cơ Sơ Kỳ" },
        { RealmType.TrucCo_TK, "Trúc Cơ Trung Kỳ" },
        { RealmType.TrucCo_HK, "Trúc Cơ Hậu Kỳ" },
        { RealmType.TrucCo_DVM, "Trúc Cơ Đại Viên Mãn" },
        { RealmType.KetDan_SK, "Kết Đan Sơ Kỳ" },
        { RealmType.KetDan_TK, "Kết Đan Trung Kỳ" },
        { RealmType.KetDan_HK, "Kết Đan Hậu Kỳ" },
        { RealmType.KetDan_DVM, "Kết Đan Đại Viên Mãn" },
        { RealmType.NguyenAnh_SK, "Nguyên Anh Sơ Kỳ" },
        { RealmType.NguyenAnh_TK, "Nguyên Anh Trung Kỳ" },
        { RealmType.NguyenAnh_HK, "Nguyên Anh Hậu Kỳ" },
        { RealmType.NguyenAnh_DVM, "Nguyên Anh Đại Viên Mãn" },
        { RealmType.HoaThan_SK, "Hóa Thần Sơ Kỳ" },
        { RealmType.HoaThan_TK, "Hóa Thần Trung Kỳ" },
        { RealmType.HoaThan_HK, "Hóa Thần Hậu Kỳ" },
        { RealmType.HoaThan_DVM, "Hóa Thần Đại Viên Mãn" },
        { RealmType.HopThe_SK, "Hợp Thể Sơ Kỳ" },
        { RealmType.HopThe_TK, "Hợp Thể Trung Kỳ" },
        { RealmType.HopThe_HK, "Hợp Thể Hậu Kỳ" },
        { RealmType.HopThe_DVM, "Hợp Thể Đại Viên Mãn" },
        { RealmType.DoKiep_SK, "Độ Kiếp Sơ Kỳ" },
        { RealmType.DoKiep_TK, "Độ Kiếp Trung Kỳ" },
        { RealmType.DoKiep_HK, "Độ Kiếp Hậu Kỳ" },
        { RealmType.DoKiep_DVM, "Độ Kiếp Đại Viên Mãn" },
        { RealmType.DaiThua_SK, "Đại Thừa Sơ Kỳ" },
        { RealmType.DaiThua_TK, "Đại Thừa Trung Kỳ" },
        { RealmType.DaiThua_HK, "Đại Thừa Hậu Kỳ" },
        { RealmType.DaiThua_DVM, "Đại Thừa Đại Viên Mãn" },
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
        { QuanlityType.Mortal, "Phàm" },
        { QuanlityType.Yellow, "Hoàng" },
        { QuanlityType.Mystic, "Huyền" },
        { QuanlityType.Earth, "Địa" },
        { QuanlityType.Heaven, "Thiên" },

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
    private static readonly Dictionary<Enum, string> _translationsAcronym = new Dictionary<Enum, string>
    {
        // CultivationStage
        { RealmType.LuyenKhi_1, "LK1" },
        { RealmType.LuyenKhi_2, "LK2" },
        { RealmType.LuyenKhi_3, "LK3" },
        { RealmType.LuyenKhi_4, "LK4" },
        { RealmType.LuyenKhi_5, "LK5" },
        { RealmType.LuyenKhi_6, "LK6" },
        { RealmType.LuyenKhi_7, "LK7" },
        { RealmType.LuyenKhi_8, "LK8" },
        { RealmType.LuyenKhi_9, "LK9" },
        { RealmType.TrucCo_SK, "TCS" },
        { RealmType.TrucCo_TK, "TCT" },
        { RealmType.TrucCo_HK, "TCH" },
        { RealmType.TrucCo_DVM, "TCĐ" },
        { RealmType.KetDan_SK, "KDS" },
        { RealmType.KetDan_TK, "KDT" },
        { RealmType.KetDan_HK, "KDH" },
        { RealmType.KetDan_DVM, "KDĐ" },
        { RealmType.NguyenAnh_SK, "NAS" },
        { RealmType.NguyenAnh_TK, "NAT" },
        { RealmType.NguyenAnh_HK, "NAH" },
        { RealmType.NguyenAnh_DVM, "NAĐ" },
        { RealmType.HoaThan_SK, "HTS" },
        { RealmType.HoaThan_TK, "HTT" },
        { RealmType.HoaThan_HK, "HTH" },
        { RealmType.HoaThan_DVM, "HTĐ" },
        { RealmType.HopThe_SK, "HTS" },
        { RealmType.HopThe_TK, "HTT" },
        { RealmType.HopThe_HK, "HTH" },
        { RealmType.HopThe_DVM, "HTĐ" },
        { RealmType.DoKiep_SK, "ĐKS" },
        { RealmType.DoKiep_TK, "ĐKT" },
        { RealmType.DoKiep_HK, "ĐKH" },
        { RealmType.DoKiep_DVM, "ĐKĐ" },
        { RealmType.DaiThua_SK, "DTS" },
        { RealmType.DaiThua_TK, "DTT" },
        { RealmType.DaiThua_HK, "DTH" },
        { RealmType.DaiThua_DVM, "DTĐ" },
        { RealmType.PhiThang, "PT" },

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
        { QuanlityType.Mortal, "Phàm" },
        { QuanlityType.Yellow, "Hoàng" },
        { QuanlityType.Mystic, "Huyền" },
        { QuanlityType.Earth, "Địa" },
        { QuanlityType.Heaven, "Thiên" },

        { PillType.Breakthrough, "Đột phá" },
        { PillType.Cultivation, "Tu luyện" },
        { PillType.Recovery, "Hồi phục" },
        { PillType.Buff, "Tăng cường" },
        { PillType.Special, "Đặc biệt" },

        // TechniqueType
        { TechniqueType.PhamNhanLuyenLinhQuyet, "PN" },
        { TechniqueType.LinhVanQuyet, "LV" },
        { TechniqueType.YeuLinhQuyet, "YL" },
        { TechniqueType.ManNguuBiPhap, "MN" },

        // SkillType
        { SkillType.DonTram, "DT" },
        { SkillType.LinhTram, "LT" },
        { SkillType.LienKichChiThuat, "LKC" },
        { SkillType.ToanLucNhatKich, "TLNK" },
        { SkillType.NhamChuan, "NC" },
        { SkillType.LinhTien, "LT" },
        { SkillType.VanLinhTien, "VLT" },
        { SkillType.VuTien, "VT" },
    };
    public static string ToVietnamese(Enum value)
    {
        if (_translations.TryGetValue(value, out var result))
            return result;
        return value.ToString();
    }
    public static string ToVietnameseAcronym(Enum value)
    {
        if (_translationsAcronym.TryGetValue(value, out var result))
            return result;
        return value.ToString();
    }
}
