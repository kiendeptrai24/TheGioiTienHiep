using System;
using TGTH.Mobile;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSkillDetailPageView : IItemDetailPageView
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI skillnameTxt;
    [SerializeField] private TextMeshProUGUI realmTxt;
    [SerializeField] private TextMeshProUGUI descriptionTxt;
    [SerializeField] private TextMeshProUGUI effectDescriptionTxt;
    [Space]
    [SerializeField] private Image nextItemIcon;
    [SerializeField] private TextMeshProUGUI nextSkillnameTxt;
    [SerializeField] private TextMeshProUGUI nextRealmTxt;
    [SerializeField] private TextMeshProUGUI nextDescriptionTxt;
    [SerializeField] private TextMeshProUGUI nextEffectDescriptionTxt;
    protected override void Awake()
    {
        base.Awake();
    }
    public override void HandleItemClicked(InventoryItem inventoryItem)
    {
        if (inventoryItem.data is SkillData skill)
        {
            itemIcon.sprite = skill.itemIcon;
            skillnameTxt.text = skill.itemName;
            realmTxt.text = skill.realm + "";
            effectDescriptionTxt.text = "+ " + skill.specialEffect;
        }
    }

}
