

using System;
using TMPro;
using UnityEngine;

public class PlayerCharacterUI : TGTHMonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthTxt;
    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private TextMeshProUGUI levelTxt;
    private StatsData stats;
    private InventoryCenterManager inventoryCenterManager;
    protected override void Awake()
    {
        base.Awake();
        stats = GetComponentInParent<StatsData>();
        inventoryCenterManager = InventoryCenterManager.Instance;

        inventoryCenterManager.OnItemPlayerChanged += OnItemPlayerChanged;
        OnItemPlayerChanged(inventoryCenterManager.playerCham);
    }

    private void OnItemPlayerChanged(ItemData data)
    {
        if (data == null) return;
        stats.SetUpItem(data);
        levelTxt.text = EnumTranslator.ToVietnameseAcronym(data.realmType);
        nameTxt.text = data.itemName;
        healthTxt.text = stats.Health.ToString();
    }

}