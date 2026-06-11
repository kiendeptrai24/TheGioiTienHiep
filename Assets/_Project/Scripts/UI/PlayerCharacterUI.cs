using DuloGames.UI;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerCharacterUI : TGTHNetworkBehaviour
{
    [SerializeField] private UIProgressBar uIHealthBar;
    [SerializeField] private TextMeshProUGUI healthTxt;
    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private TextMeshProUGUI levelTxt;

    private StatsData stats;
    private InventoryCenterManager inventoryCM;
    private ProfileManager profileManager;
    private PlayerProfile playerProfile;
    private PlayerVitals playerVitals;

    private NetworkVariable<FixedString64Bytes> PlayerName =
        new(default,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner);

    private NetworkVariable<FixedString64Bytes> PlayerLevel =
        new(default,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner);

    protected override void Awake()
    {
        base.Awake();

        playerProfile = GetComponent<PlayerProfile>();
        playerVitals = GetComponent<PlayerVitals>();
        stats = GetComponentInParent<StatsData>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        inventoryCM = InventoryCenterManager.Instance;
        profileManager = ProfileManager.Instance;

        PlayerName.OnValueChanged += OnNetworkNameChanged;
        PlayerLevel.OnValueChanged += OnNetworkLevelChanged;

        if (playerVitals != null)
            playerVitals.OnVitalChanged += OnVitalChanged;

        if (playerProfile != null)
            playerProfile.OnPlayerNameChange += OnLocalPlayerNameChanged;

        if (IsOwner)
        {
            if (profileManager != null)
            {
                profileManager.OnProfileChanged += OnProfileChanged;
                profileManager.OnProfileReady += OnProfileChanged;
            }

            if (inventoryCM != null)
            {
                inventoryCM.OnItemPlayerChanged += OnItemPlayerChanged;
                OnItemPlayerChanged(inventoryCM.playerCham);
            }

            if (profileManager != null)
            {
                var profile = profileManager.GetProfile();
                if (profile != null)
                    SetName(profile.userName);
            }
        }

        RefreshAllUI();
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        PlayerName.OnValueChanged -= OnNetworkNameChanged;
        PlayerLevel.OnValueChanged -= OnNetworkLevelChanged;

        if (playerVitals != null)
            playerVitals.OnVitalChanged -= OnVitalChanged;

        if (playerProfile != null)
            playerProfile.OnPlayerNameChange -= OnLocalPlayerNameChanged;

        if (IsOwner)
        {
            if (profileManager != null)
            {
                profileManager.OnProfileChanged -= OnProfileChanged;
                profileManager.OnProfileReady -= OnProfileChanged;
            }

            if (inventoryCM != null)
                inventoryCM.OnItemPlayerChanged -= OnItemPlayerChanged;
        }
    }

    private void RefreshAllUI()
    {
        RefreshName();
        RefreshHealth();
        RefreshLevel();
    }

    private void RefreshName()
    {
        if (!PlayerName.Value.IsEmpty)
            nameTxt.text = PlayerName.Value.ToString();
    }

    private void RefreshLevel()
    {
        if (!PlayerLevel.Value.IsEmpty)
            levelTxt.text = PlayerLevel.Value.ToString();
    }

    private void RefreshHealth()
    {
        if (playerVitals == null)
            return;

        var health = playerVitals.GetVital(VitalType.Health);
        SetHealthBar(health.max, health.current);
    }

    private void OnVitalChanged(VitalType type, int maxValue, int curValue)
    {
        if (type != VitalType.Health)
            return;

        SetHealthBar(maxValue, curValue);
    }

    private void SetHealthBar(float maxValue, float curValue)
    {
        uIHealthBar.fillAmount = GetPercent(maxValue, curValue);
        healthTxt.SetText("{0}/{1}", curValue, maxValue);
    }

    private float GetPercent(float maxValue, float curValue)
    {
        if (maxValue <= 0)
            return 0f;

        return Mathf.Clamp01(curValue / maxValue);
    }

    private void OnLocalPlayerNameChanged(string value)
    {
        SetName(value);
    }

    private void OnProfileChanged(ProfileUser user)
    {
        if (user == null)
            return;

        SetName(user.userName);
    }

    private void OnItemPlayerChanged(ItemData data)
    {
        if (!IsOwner) return;
        if (data == null) return;

        if (stats != null)
            stats.SetUpItem(data);

        string levelName = EnumTranslator.ToVietnameseAcronym(data.realmType);
        SetLevel(levelName);
    }

    private void SetName(string playerName)
    {
        if (!IsSpawned) return;
        if (!IsOwner) return;
        if (string.IsNullOrEmpty(playerName)) return;

        var fixedName = new FixedString64Bytes(playerName);

        if (PlayerName.Value.Equals(fixedName))
            return;

        PlayerName.Value = fixedName;
    }

    private void SetLevel(string levelName)
    {
        if (!IsSpawned) return;
        if (!IsOwner) return;
        if (string.IsNullOrEmpty(levelName)) return;

        var fixedLevel = new FixedString64Bytes(levelName);

        if (PlayerLevel.Value.Equals(fixedLevel))
            return;

        PlayerLevel.Value = fixedLevel;
    }

    private void OnNetworkNameChanged(FixedString64Bytes oldValue, FixedString64Bytes newValue)
    {
        nameTxt.text = newValue.ToString();
    }

    private void OnNetworkLevelChanged(FixedString64Bytes oldValue, FixedString64Bytes newValue)
    {
        levelTxt.text = newValue.ToString();
    }
}