

using System;
using TMPro;
using UnityEngine;

public class PlayerCharacterUI : TGTHMonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthTxt;
    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private TextMeshProUGUI levelTxt;
    private StatsData stats;
    private ProfileManager profileManager;
    private InventoryCenterManager inventoryCenterManager;
    protected override void Awake()
    {
        base.Awake();
        stats = GetComponentInParent<StatsData>();
        profileManager = ProfileManager.Instance;
        inventoryCenterManager = InventoryCenterManager.Instance;
        profileManager.OnProfileReady += OnProfileReady;
        inventoryCenterManager.OnItemPlayerChanged += OnItemPlayerChanged;
    }

    private void OnItemPlayerChanged(ItemData data)
    {
        stats.SetUpItem(data);
        levelTxt.text = EnumTranslator.ToVietnameseAcronym(data.realmType);
        healthTxt.text = stats.Health.ToString();
    }
    private void OnProfileReady(ProfileUser user)
    {
        nameTxt.text = user.userName;
    }

}