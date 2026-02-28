using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TGTH.Mobile
{

    public class StatsPageView : MonoBehaviour
    {
        #region Stat

        [Header("=== Charactor info ===")]
        [SerializeField] private TextMeshProUGUI cultivationStageTxt;
        [SerializeField] private TextMeshProUGUI classTxt;
        [SerializeField] private TextMeshProUGUI raceTxt;
        [SerializeField] private TextMeshProUGUI moraleTxt;
        [SerializeField] private TextMeshProUGUI authorityTxt;
        [SerializeField] private TextMeshProUGUI moveSpeedTxt;
        [SerializeField] private TextMeshProUGUI spiritRangeTxt;

        [Header("=== Main attributes ===")]
        [SerializeField] private TextMeshProUGUI potentialPointTxt;
        [SerializeField] private TextMeshProUGUI hpTxt;
        [SerializeField] private TextMeshProUGUI defenseTxt;
        [SerializeField] private TextMeshProUGUI phicialDamagePointTxt;
        [SerializeField] private TextMeshProUGUI magicalDamagePointTxt;
        [SerializeField] private TextMeshProUGUI spiritDamagePointTxt;

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
            moveSpeedTxt.text = stats[StatType.MovementSpeed].GetValue().ToString();
            spiritRangeTxt.text = stats[StatType.SpiritRange].GetValue().ToString();
            potentialPointTxt.text = stats[StatType.Potential].GetValue().ToString();
            hpTxt.text = stats[StatType.Health].GetValue().ToString();
            defenseTxt.text = stats[StatType.PhysicalDefense].GetValue().ToString();
            phicialDamagePointTxt.text = stats[StatType.PhysicalDamage].GetValue().ToString();
            magicalDamagePointTxt.text = stats[StatType.MagicalDamage].GetValue().ToString();
            spiritDamagePointTxt.text = stats[StatType.SpiritDamage].GetValue().ToString();

            physicalDamageTxt.text = stats[StatType.PhysicalDamage].GetValue().ToString();
            magicalDamageTxt.text = stats[StatType.MagicalDamage].GetValue().ToString();
            spiritDamageTxt.text = stats[StatType.SpiritDamage].GetValue().ToString();
            critPowerTxt.text = stats[StatType.CritPower].GetValue().ToString();
            critChanceTxt.text = stats[StatType.CritChance].GetValue().ToString();
            attackSpeedTxt.text = stats[StatType.AttackSpeed].GetValue().ToString();
            trueDamageTxt.text = stats[StatType.TrueDamage].GetValue().ToString();
            armorPenetrationTxt.text = stats[StatType.ArmorPenetration].GetValue().ToString();
            lifeStealTxt.text = stats[StatType.LifeSteal].GetValue().ToString();

            healthTxt.text = stats[StatType.Health].GetValue().ToString();
            manaTxt.text = stats[StatType.Mana].GetValue().ToString();
            spiritTxt.text = stats[StatType.Spirit].GetValue().ToString();
            healthRegenTxt.text = stats[StatType.HealthRegen].GetValue().ToString();
            manaRegenTxt.text = stats[StatType.ManaRegen].GetValue().ToString();
            physicalDefenseTxt.text = stats[StatType.PhysicalDefense].GetValue().ToString();
            magicalDefenseTxt.text = stats[StatType.MagicalDefense].GetValue().ToString();
            spiritDefenseTxt.text = stats[StatType.SpiritDefense].GetValue().ToString();
            critDamageReductionTxt.text = stats[StatType.CritDamageReduction].GetValue().ToString();
        }
        public void ShowCharactorIdentifyData(CharacterIdentity characterIdentity)
        {
            cultivationStageTxt.text = characterIdentity.cultivationStage.ToString();
            classTxt.text = characterIdentity.essenceType.ToString();
            raceTxt.text = characterIdentity.raceType.ToString();
        }
    }
}

