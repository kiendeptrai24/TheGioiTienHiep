using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TGTH.Mobile
{
    public class StatsPageView : MonoBehaviour
    {
        #region Stat

        [Header("=== Character info ===")]
        [SerializeField] private TextMeshProUGUI realmTxt;
        [SerializeField] private TextMeshProUGUI classTxt;
        [SerializeField] private TextMeshProUGUI raceTxt;
        [SerializeField] private TextMeshProUGUI moraleTxt;
        [SerializeField] private TextMeshProUGUI authorityTxt;
        [SerializeField] private TextMeshProUGUI moveSpeedTxt;
        [SerializeField] private TextMeshProUGUI spiritRangeTxt;

        [Header("=== Main attributes ===")]
        [SerializeField] private TextMeshProUGUI potentialPointTxt;
        [SerializeField] private TextMeshProUGUI skillPointTxt;
        [SerializeField] private TextMeshProUGUI damagePointTxt;
        [SerializeField] private TextMeshProUGUI defensePointTxt;
        [SerializeField] private TextMeshProUGUI healthPointTxt;
        [SerializeField] private TextMeshProUGUI ManaPointTxt;
        [SerializeField] private TextMeshProUGUI spiritPointTxt;

        [Header("=== Combat Index - Left ===")]
        [SerializeField] private TextMeshProUGUI physicalDamageTxt;
        [SerializeField] private TextMeshProUGUI magicalDamageTxt;
        [SerializeField] private TextMeshProUGUI spiritDamageTxt;
        [SerializeField] private TextMeshProUGUI critPowerTxt;
        [SerializeField] private TextMeshProUGUI critChanceTxt;
        [SerializeField] private TextMeshProUGUI attackSpeedTxt;
        [SerializeField] private TextMeshProUGUI trueDamageTxt;
        [SerializeField] private TextMeshProUGUI armorPenetrationTxt;
        [SerializeField] private TextMeshProUGUI lifeStealTxt;

        [Header("=== Combat Index - Right ===")]
        [SerializeField] private TextMeshProUGUI healthTxt;
        [SerializeField] private TextMeshProUGUI manaTxt;
        [SerializeField] private TextMeshProUGUI spiritTxt;
        [SerializeField] private TextMeshProUGUI healthRegenTxt;
        [SerializeField] private TextMeshProUGUI manaRegenTxt;
        [SerializeField] private TextMeshProUGUI physicalDefenseTxt;
        [SerializeField] private TextMeshProUGUI magicalDefenseTxt;
        [SerializeField] private TextMeshProUGUI spiritDefenseTxt;
        [SerializeField] private TextMeshProUGUI critDamageReductionTxt;
        #endregion
        public void SetStatsData(Dictionary<StatType, Stat> stats)
        {
            var profile = ProfileManager.Instance != null ? ProfileManager.Instance.GetProfile() : null;

            if (potentialPointTxt != null)
            {
                potentialPointTxt.text = profile != null ? profile.potentialPoint.ToString() : "0";
            }
            if (skillPointTxt != null)
            {
                skillPointTxt.text = profile != null ? profile.skillPoint.ToString() : "0";
            }

            damagePointTxt.text = stats[StatType.PhicialDamagePoint].GetValue().ToString();
            defensePointTxt.text = stats[StatType.PhicialDefensePoint].GetValue().ToString();

            healthPointTxt.text = stats[StatType.HealthPoint].GetValue().ToString();
            ManaPointTxt.text = stats[StatType.ManaPoint].GetValue().ToString();
            spiritPointTxt.text = stats[StatType.SpiritPoint].GetValue().ToString();

            spiritRangeTxt.text = stats[StatType.SpiritRangePoint].GetValue().ToString();
            moveSpeedTxt.text = stats[StatType.MoveSpeedPoint].GetValue().ToString();

            physicalDamageTxt.text = stats[StatType.PhysicalDamage].GetValue().ToString();
            magicalDamageTxt.text = stats[StatType.MagicalDamage].GetValue().ToString();
            spiritDamageTxt.text = stats[StatType.SpiritDamage].GetValue().ToString();

            critPowerTxt.text = stats[StatType.CritPower].GetValue().ToString();
            critChanceTxt.text = stats[StatType.CritChance].GetValue().ToString();

            attackSpeedTxt.text = stats[StatType.AttackSpeed].GetValue().ToString();
            trueDamageTxt.text = stats[StatType.TrueDamage].GetValue().ToString();

            armorPenetrationTxt.text = stats[StatType.ArmorPenetration].GetValue().ToString();
            lifeStealTxt.text = stats[StatType.LifeSteal].GetValue().ToString();
            float maxHealth = stats[StatType.Health].GetValue();
            float maxMana = stats[StatType.Mana].GetValue();
            float maxSpirit = stats[StatType.Spirit].GetValue();

            float curHealth = maxHealth;
            float curMana = maxMana;
            float curSpirit = maxSpirit;

            if (profile != null)
            {
                curHealth = profile.currentHealth;
                curMana = profile.currentMana;
                curSpirit = profile.currentSpirit;
            }

            healthTxt.text = $"{curHealth}/{maxHealth}";
            manaTxt.text = $"{curMana}/{maxMana}";
            spiritTxt.text = $"{curSpirit}/{maxSpirit}";

            healthRegenTxt.text = stats[StatType.HealthRegen].GetValue().ToString();
            manaRegenTxt.text = stats[StatType.ManaRegen].GetValue().ToString();

            physicalDefenseTxt.text = stats[StatType.PhysicalDefense].GetValue().ToString();
            magicalDefenseTxt.text = stats[StatType.MagicalDefense].GetValue().ToString();
            spiritDefenseTxt.text = stats[StatType.SpiritDefense].GetValue().ToString();

            critDamageReductionTxt.text = stats[StatType.CritDamageReduction].GetValue().ToString();
        }
        public void ShowCharacterIdentifyData(ItemData itemData)
        {
            HeroData heroData = itemData as HeroData;
            if (heroData == null) return;

            realmTxt.text = EnumTranslator.ToVietnamese(heroData.essenceData.realmType);
            classTxt.text = EnumTranslator.ToVietnamese(heroData.essenceType);
            raceTxt.text = EnumTranslator.ToVietnamese(heroData.raceType);
        }
    }
}

