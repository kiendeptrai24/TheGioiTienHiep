using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemTechniqueDetailPageView : IItemDetailPageView
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI techniquenameTxt;
    [SerializeField] private TextMeshProUGUI realmTxt;
    [SerializeField] private TextMeshProUGUI descriptionTxt;
    [SerializeField] private TextMeshProUGUI effectDescriptionTxt;
    [Space]
    [SerializeField] private Image nextItemIcon;
    [SerializeField] private TextMeshProUGUI nextTechniquenameTxt;
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
            techniquenameTxt.text = skill.itemName;
            realmTxt.text = skill.realm + "";
            effectDescriptionTxt.text = "+ " + skill.specialEffect;
        }
    }

}
