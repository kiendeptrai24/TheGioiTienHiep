
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ProfileManager : Singleton<ProfileManager>, ISaveable
{
    [SerializeField] private ProfileUser profileUser;
    public List<ulong> resourceIds = new List<ulong>();
    public event Action<ProfileUser> OnProfileChanged;
    public event Action<ProfileUser> OnProfileReady;
    private PlayerNetManager playerNM;
    protected override void Awake()
    {
        base.Awake();

        playerNM = PlayerNetManager.Instance;
        playerNM.OnPlayerExiststed += OnPlayerExiststed;

        string userId = "";
        profileUser = new ProfileUser(userId, "người chơi");

        NotiProfileChanged();
    }
    private void OnPlayerExiststed(NetworkObject playerNet)
    {
        var relinker = playerNet.GetComponent<PlayerMineRelinker>();
        var playerProfile = playerNet.GetComponent<PlayerProfile>();
        playerProfile.OnProfileChanged += NotiProfileChanged;
        relinker.OnResourceIdsChanged += OnResourceIdsChanged;
    }
    private void OnResourceIdsChanged(List<ulong> list) => resourceIds = list;
    private void NotiProfileChanged() => OnProfileChanged?.Invoke(profileUser);

    public ProfileUser GetProfile() => profileUser;

    public void SetProfileUser(string userName)
    {
        profileUser.userName = userName;
        NotiProfileChanged();
    }
    public void AddFriend(string friend)
    {
        profileUser.AddFriend(friend);
    }
    public void RemoveFriend(string friend)
    {
        profileUser.RemoveFriend(friend);
    }
    #region SaveLoadData

    public void LoadData(GameData _data)
    {
        profileUser.userName = _data.characterName;
        profileUser.coins = _data.coins;
        profileUser.userId = _data.characterId;
        profileUser.potentialPoint = _data.potentialPoint;
        profileUser.skillPoint = _data.skillPoint;
        profileUser.itemDataPoint = _data.itemDataPoint;
        profileUser.playerResource.linhThach = (int)_data.coins;
        OnProfileReady?.Invoke(profileUser);
    }

    public void SaveGame(ref GameData _data)
    {
        _data.mineOfflineDataList.Clear();
        _data.coins = profileUser.coins;
        _data.characterName = profileUser.userName;
        _data.potentialPoint = profileUser.potentialPoint;
        _data.skillPoint = profileUser.skillPoint;
        _data.itemDataPoint = profileUser.itemDataPoint;
        foreach (var resourceId in resourceIds)
        {
            _data.mineOfflineDataList.AddOrUpdate(resourceId, 0, 0, "");
        }
        for (int i = 0; i < _data.itemCharacterDatas.Count; i++)
        {
            var heroData = _data.itemCharacterDatas[i] as HeroData;
            if (heroData != null && heroData.characterId == profileUser.userId)
            {
                heroData.itemName = profileUser.userName;
                _data.characterName = profileUser.userName;
                break;
            }
        }
    }
    #endregion
}