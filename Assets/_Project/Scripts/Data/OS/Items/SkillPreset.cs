#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(fileName = "NewSkillPreset", menuName = "RPG/Items/Skill Preset")]
public class SkillPreset : ItemPreset
{
    [Header("Damage/Defense Stats (%)")]
    public float physicalDamage;   // % Sát thương linh thể
    public float magicalDamage;    // % Sát thương linh lực
    public float spiritDamage;     // % Sát thương linh thức

    public float physicalDefense;  // % Phòng ngự linh thể
    public float magicalDefense;   // % Phòng ngự linh lực
    public float spiritDefense;    // % Phòng ngự linh thức

    [Header("Meta")]
    public SkillType skillType;
    public int enhanceLevel;
    public RaceType raceType;
    public EssenceType mainEssence;
    public ElementType elementType;
    public CultivationStage realm;

    [Header("Combat")]
    public float attackRange;
    public float cooldown;
    [TextArea] public string specialEffect;

    [Header("Resource Cost")]
    public float healthCost;
    public float manaCost;
    public float spiritCost;

    [Header("Learn Conditions")]
    public int requiredCharacterLevel;
    public string learnCondition;
    public string otherNote;

    [Header("Upgrade Materials")]
    public float powerCost;
    public float lthaoCost;
    public float mineralCost;
    public float demonCoreCost;
    public float devilCoreCost;
    public float spiritStoneCost;
    public float itemCost;

    [Header("Bonus Stats (%)")]
    public float critDamage;
    public float critRate;

    public float armorPenetration;
    public float trueDamage;
    public float lifeSteal;
    public float attackSpeed;

    [Header("Damage Reduction (%)")]
    public float penetrationReduction;
    public float critDamageReduction;
    public float trueDamageReduction;

    [Header("Resource Bonus (%)")]
    public float bonusHealth;
    public float bonusMana;
    public float bonusSpirit;

    [Header("Summary")]
    public float totalQualityAndLevel;
    public int statCount;

    [ContextMenu("Reset To Default")]
    public void ResetToDefault()
    {
        ClearStats();
        ApplyCommonMetaDefaults();

        switch (skillType)
        {
            case SkillType.DonTram:
                ApplyDonTram();
                break;

            case SkillType.LinhTram:
                ApplyLinhTram();
                break;

            case SkillType.LienKichChiThuat:
                ApplyLienKich();
                break;

            case SkillType.ToanLucNhatKich:
                ApplyToanLucNhatKich();
                break;

            case SkillType.NhamChuan:
                ApplyNhamChuan();
                break;

            case SkillType.LinhTien:
                ApplyLinhTien();
                break;

            case SkillType.VanLinhTien:
                ApplyVanLinhTien();
                break;

            case SkillType.VuTien:
                ApplyVuTien();
                break;
        }

        // auto itemName theo skillType + enhance
        itemName = $"{GetSkillDisplayName(skillType)} C{enhanceLevel}";

#if UNITY_EDITOR
        string newName = $"Skill_{itemName}";
        if (name != newName)
        {
            string path = AssetDatabase.GetAssetPath(this);
            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.RenameAsset(path, newName);
                AssetDatabase.SaveAssets();
            }
        }
#endif
    }

    // ----------------------------
    // Helpers
    // ----------------------------
    private void ApplyCommonMetaDefaults()
    {
        // theo bảng bạn: đa số là Phàm/Chung/Chung/Chung
        if (qualityType == default) qualityType = QualityType.Mortal;
        if (raceType == default) raceType = RaceType.General;
        if (mainEssence == default) mainEssence = EssenceType.General;
        if (elementType == default) elementType = ElementType.Neutral;
        if (string.IsNullOrEmpty(learnCondition)) learnCondition = "Không";
    }

    private void ClearStats()
    {
        physicalDamage = magicalDamage = spiritDamage = 0;
        physicalDefense = magicalDefense = spiritDefense = 0;

        critDamage = critRate = 0;
        armorPenetration = trueDamage = lifeSteal = attackSpeed = 0;

        penetrationReduction = critDamageReduction = trueDamageReduction = 0;
        bonusHealth = bonusMana = bonusSpirit = 0;

        attackRange = 0;
        cooldown = 0;
        specialEffect = string.Empty;

        healthCost = manaCost = spiritCost = 0;

        requiredCharacterLevel = 0;
        otherNote = string.Empty;

        powerCost = lthaoCost = mineralCost = demonCoreCost = devilCoreCost = spiritStoneCost = itemCost = 0;

        statCount = 0;
        totalQualityAndLevel = 0;
        enhanceLevel = 0;
    }

    private static string GetSkillDisplayName(SkillType t)
    {
        return t switch
        {
            SkillType.DonTram => "Đơn Trảm",
            SkillType.LinhTram => "Linh Trảm",
            SkillType.LienKichChiThuat => "Liên Kích Chi Thuật",
            SkillType.ToanLucNhatKich => "Toàn Lực Nhất Kích",
            SkillType.NhamChuan => "Nhắm Chuẩn",
            SkillType.LinhTien => "Linh Tiễn",
            SkillType.VanLinhTien => "Vận Lịnh Tiễn",
            SkillType.VuTien => "Vũ Tiễn",
            _ => t.ToString()
        };
    }

    // map “cost theo cảnh giới” bạn paste: 9/13/25/50/150/350/650/1250/2500
    private void ApplyCommonHealthCostByRealm()
    {
        healthCost = realm switch
        {
            CultivationStage.LuyenKhi_9 => 9,
            CultivationStage.TrucCo_SK => 13,
            CultivationStage.KetDan_SK => 25,
            CultivationStage.NguyenAnh_SK => 50,
            CultivationStage.HoaThan_SK => 150,
            CultivationStage.HopThe_SK => 350,
            CultivationStage.DoKiep_SK => 650,
            CultivationStage.DaiThua_SK => 1250,
            CultivationStage.PhiThang => 2500,
            _ => healthCost
        };
    }

    private int EnhanceFromRealm()
    {
        return realm switch
        {
            CultivationStage.LuyenKhi_9 => 1,
            CultivationStage.TrucCo_SK => 2,
            CultivationStage.KetDan_SK => 3,
            CultivationStage.NguyenAnh_SK => 4,
            CultivationStage.HoaThan_SK => 5,
            CultivationStage.HopThe_SK => 6,
            CultivationStage.DoKiep_SK => 7,
            CultivationStage.DaiThua_SK => 8,
            CultivationStage.PhiThang => 9,
            _ => 1
        };
    }

    // ----------------------------
    // APPLY PER SKILL (theo bảng bạn gửi)
    // ----------------------------

    private void ApplyDonTram()
    {
        attackRange = 1;
        cooldown = 5;
        ApplyCommonHealthCostByRealm();
        enhanceLevel = EnhanceFromRealm();

        physicalDamage = realm switch
        {
            CultivationStage.LuyenKhi_9 => 20,
            CultivationStage.TrucCo_SK => 30,
            CultivationStage.KetDan_SK => 50,
            CultivationStage.NguyenAnh_SK => 70,
            CultivationStage.HoaThan_SK => 90,
            CultivationStage.HopThe_SK => 110,
            CultivationStage.DoKiep_SK => 130,
            CultivationStage.DaiThua_SK => 160,
            CultivationStage.PhiThang => 210,
            _ => 0
        };

        specialEffect = $"Sát thương linh thể là {physicalDamage:0.##}%";
        statCount = 1;
    }

    private void ApplyLinhTram()
    {
        attackRange = 1;
        cooldown = 8;
        ApplyCommonHealthCostByRealm();
        enhanceLevel = EnhanceFromRealm();

        // bảng bạn có 3 mức: LK9 / Trúc Cơ / Kết Đan
        if (realm == CultivationStage.LuyenKhi_9)
        {
            physicalDamage = 15;
            physicalDefense = 5;
        }
        else if (realm == CultivationStage.TrucCo_SK)
        {
            physicalDamage = 23;
            physicalDefense = 7;
        }
        else if (realm == CultivationStage.KetDan_SK)
        {
            physicalDamage = 35;
            physicalDefense = 15;
        }

        specialEffect = $"Sát thương linh thể là {physicalDamage:0.##}%\nPhòng ngự linh thể là {physicalDefense:0.##}%";
        statCount = 2;
    }

    private void ApplyLienKich()
    {
        attackRange = 1;
        cooldown = 10;
        ApplyCommonHealthCostByRealm();
        enhanceLevel = EnhanceFromRealm();

        if (realm == CultivationStage.LuyenKhi_9) physicalDamage = 20;
        else if (realm == CultivationStage.TrucCo_SK) physicalDamage = 30;
        else if (realm == CultivationStage.KetDan_SK) physicalDamage = 50;

        specialEffect = $"Sát thương linh thể là {physicalDamage:0.##}%";
        statCount = 1;
    }

    private void ApplyToanLucNhatKich()
    {
        attackRange = 1;
        cooldown = 13;
        ApplyCommonHealthCostByRealm();
        enhanceLevel = EnhanceFromRealm();

        if (realm == CultivationStage.LuyenKhi_9)
        {
            physicalDamage = 17;
            critDamage = 1.5f; // theo cột số bạn paste
        }
        else if (realm == CultivationStage.TrucCo_SK)
        {
            physicalDamage = 25;
            critDamage = 2.5f;
        }
        else if (realm == CultivationStage.KetDan_SK)
        {
            physicalDamage = 40;
            critDamage = 5f;
        }

        specialEffect = $"Sát thương linh thể là {physicalDamage:0.##}%\nSát thương chí mạng là {critDamage:0.##}%";
        statCount = 2;
    }

    private void ApplyNhamChuan()
    {
        attackRange = 0;
        cooldown = 8;
        ApplyCommonHealthCostByRealm();
        enhanceLevel = EnhanceFromRealm();

        if (realm == CultivationStage.LuyenKhi_9) critRate = 10;
        else if (realm == CultivationStage.TrucCo_SK) critRate = 15;
        else if (realm == CultivationStage.KetDan_SK) critRate = 25;

        specialEffect = $"Tỷ lệ chí mạng là {critRate:0.##}%";
        statCount = 1;
    }

    private void ApplyLinhTien()
    {
        attackRange = 3;
        cooldown = 5;
        ApplyCommonHealthCostByRealm();
        enhanceLevel = EnhanceFromRealm();

        if (realm == CultivationStage.LuyenKhi_9) magicalDamage = 20;
        else if (realm == CultivationStage.TrucCo_SK) magicalDamage = 30;
        else if (realm == CultivationStage.KetDan_SK) magicalDamage = 50;

        specialEffect = $"Sát thương linh lực là {magicalDamage:0.##}%";
        statCount = 1;
    }

    private void ApplyVanLinhTien()
    {
        attackRange = 3;
        cooldown = 10;
        ApplyCommonHealthCostByRealm();
        enhanceLevel = EnhanceFromRealm();

        // bảng bạn: magicalDamage 15/25/45, critDamage 2.5, (mình set critRate = 5 theo cột 5% bạn paste)
        if (realm == CultivationStage.LuyenKhi_9) magicalDamage = 15;
        else if (realm == CultivationStage.TrucCo_SK) magicalDamage = 25;
        else if (realm == CultivationStage.KetDan_SK) magicalDamage = 45;

        critDamage = 2.5f;
        critRate = 5f;

        specialEffect = $"Sát thương linh lực là {magicalDamage:0.##}%\nSát thương chí mạng là {critDamage:0.##}%";
        statCount = 2;
    }

    private void ApplyVuTien()
    {
        attackRange = 3;
        cooldown = 13;
        ApplyCommonHealthCostByRealm();
        enhanceLevel = EnhanceFromRealm();

        if (realm == CultivationStage.LuyenKhi_9)
        {
            magicalDamage = 10;
            critDamage = 5;
            critRate = 10;
        }
        else if (realm == CultivationStage.TrucCo_SK)
        {
            magicalDamage = 17;
            critDamage = 6.5f;
            critRate = 13;
        }
        else if (realm == CultivationStage.KetDan_SK)
        {
            magicalDamage = 32;
            critDamage = 9;
            critRate = 18;
        }

        specialEffect = $"Sát thương linh lực là {magicalDamage:0.##}%\nSát thương chí mạng là {critDamage:0.##}%";
        statCount = 3;
    }

    // (Nếu bạn muốn) export ra ItemData runtime:
    public override ItemData GetItemData()
    {
        return new SkillData
        {
            // base
            itemId = itemId,
            itemName = itemName,
            itemType = itemType,
            itemIcon = itemIcon,
            itemDescription = itemDescription,
            currentstack = currentstack,

            // base stats trong ItemData
            physicalDamage = physicalDamage,
            magicalDamage = magicalDamage,
            spiritDamage = spiritDamage,
            physicalDefense = physicalDefense,
            magicalDefense = magicalDefense,
            spiritDefense = spiritDefense,

            // meta
            skillType = skillType,
            qualityType = qualityType,
            enhanceLevel = enhanceLevel,
            raceType = raceType,
            mainEssence = mainEssence,
            elementType = elementType,
            realm = realm,

            // combat
            attackRange = attackRange,
            cooldown = cooldown,
            specialEffect = specialEffect,

            // costs
            healthCost = healthCost,
            manaCost = manaCost,
            spiritCost = spiritCost,

            // learn
            requiredCharacterLevel = requiredCharacterLevel,
            learnCondition = learnCondition,
            otherNote = otherNote,

            // materials
            powerCost = powerCost,
            lthaoCost = lthaoCost,
            mineralCost = mineralCost,
            demonCoreCost = demonCoreCost,
            devilCoreCost = devilCoreCost,
            spiritStoneCost = spiritStoneCost,
            itemCost = itemCost,

            // bonus
            critDamage = critDamage,
            critRate = critRate,
            armorPenetration = armorPenetration,
            trueDamage = trueDamage,
            lifeSteal = lifeSteal,
            attackSpeed = attackSpeed,

            penetrationReduction = penetrationReduction,
            critDamageReduction = critDamageReduction,
            trueDamageReduction = trueDamageReduction,

            bonusHealth = bonusHealth,
            bonusMana = bonusMana,
            bonusSpirit = bonusSpirit,

            totalQualityAndLevel = totalQualityAndLevel,
            statCount = statCount
        };
    }
}
