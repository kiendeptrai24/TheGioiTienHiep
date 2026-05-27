

using System;
using DuloGames.UI;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerCharacterUI : TGTHNetworkBehaviour
{
    [SerializeField] private UIProgressBar uIProgressBar;
    [SerializeField] private TextMeshProUGUI healthTxt;
    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private TextMeshProUGUI levelTxt;
    private StatsData stats;
    private InventoryCenterManager inventoryCenterManager;
    private ProfileManager profileManager;
    private NetworkVariable<FixedString64Bytes> PlayerName =
    new NetworkVariable<FixedString64Bytes>();

    protected override void Awake()
    {
        base.Awake();
    }
    private void OnProfileChanged(ProfileUser user)
    {
        SetName(user.userName);
    }
    private void SetProcessBar(float max, float cur)
    {
        uIProgressBar.fillAmount = cur / max;
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        PlayerName.OnValueChanged += OnPlayerNameChanged;
        SetName(nameTxt.text);
        stats = GetComponentInParent<StatsData>();
        inventoryCenterManager = InventoryCenterManager.Instance;
        profileManager = ProfileManager.Instance;

        profileManager.OnProfileChanged += OnProfileChanged;
        profileManager.OnProfileReady += OnProfileChanged;
        inventoryCenterManager.OnItemPlayerChanged += OnItemPlayerChanged;
        OnItemPlayerChanged(inventoryCenterManager.playerCham);
    }

    private void OnPlayerNameChanged(FixedString64Bytes previousValue, FixedString64Bytes newValue)
    {
        nameTxt.text = newValue.ToString();
    }

    public void SetName(string name)
    {
        if (IsSpawned)
        {
            PlayerName.Value = name;
        }
    }
    private void OnItemPlayerChanged(ItemData data)
    {
        if (data == null) return;
        stats.SetUpItem(data);
        levelTxt.text = EnumTranslator.ToVietnameseAcronym(data.realmType);
        nameTxt.text = data.itemName;
        healthTxt.text = stats.Health.ToString();
        SetProcessBar(stats.Health, stats.Health);
    }

}