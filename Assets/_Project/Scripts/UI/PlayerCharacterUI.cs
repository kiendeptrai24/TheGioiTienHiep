

using System;
using DuloGames.UI;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerCharacterUI : TGTHNetworkBehaviour
{
    [SerializeField] private UIProgressBar uIHealthBar;
    [SerializeField] private UIProgressBar uIManaBar;
    [SerializeField] private UIProgressBar uIPiritBar;
    [SerializeField] private TextMeshProUGUI healthTxt;
    [SerializeField] private TextMeshProUGUI manaTxt;
    [SerializeField] private TextMeshProUGUI spiritTxt;
    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private TextMeshProUGUI levelTxt;
    private StatsData stats;
    private InventoryCenterManager inventoryCenterManager;
    private ProfileManager profileManager;
    private NetworkVariable<FixedString64Bytes> PlayerName =
    new NetworkVariable<FixedString64Bytes>(
        default,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );
    protected override void Awake()
    {
        base.Awake();
        PlayerProfile profile = GetComponent<PlayerProfile>();
        profile.OnPlayerNameChange += OnPlayerNameChanged;
        PlayerVitals playerVitals = GetComponent<PlayerVitals>();
        playerVitals.OnVitalChanged += OnVitalChanged;
    }

    private void OnVitalChanged(VitalType type, int maxValue, int curValue)
    {
        switch (type)
        {
            case VitalType.Health:
                SetHealthBar(maxValue, curValue);
                break;
            case VitalType.Mana:
                SetManaBar(maxValue, curValue);
                break;
            case VitalType.Spirit:
                SetSpiritBar(maxValue, curValue);
                break;
        }
    }

    private void SetSpiritBar(int maxValue, int curValue)
    {
        uIPiritBar.fillAmount = (float)curValue / (float)maxValue;
        spiritTxt.text = curValue.ToString();
    }

    private void SetManaBar(int maxValue, int curValue)
    {
        uIManaBar.fillAmount = (float)curValue / (float)maxValue;
        manaTxt.text = curValue.ToString();
    }
    private void SetHealthBar(float maxValue, float curValue)
    {
        uIHealthBar.fillAmount = (float)curValue / (float)maxValue;
        healthTxt.text = curValue.ToString();
    }

    private void OnPlayerNameChanged(string value)
    {
        nameTxt.text = value;
    }

    private void OnProfileChanged(ProfileUser user)
    {
        SetName(user.userName);
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
        if (IsSpawned && IsOwner)
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
        healthTxt.text = data.health.ToString();
        SetHealthBar(data.health, data.health);
    }

}