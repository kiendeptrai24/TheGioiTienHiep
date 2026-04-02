
using System;
using System.Collections.Generic;
using UnityEngine;

public class ProfileManager : Singleton<ProfileManager>, ISaveable
{
    [SerializeField] private ProfileUser profileUser;
    public List<ulong> resourceIds = new List<ulong>();
    public event Action<ProfileUser> OnProfileCoinsChanged;
    public event Action<ProfileUser> OnProfileChanged;
    public event Action<ProfileUser> OnProfileReady;
    protected override void Awake()
    {
        base.Awake();
        string userId = "";
        profileUser = new ProfileUser(userId, "người chơi");
    }
    public void BindResource(ResourceStorage storage)
    {
        storage.OnCoinsChanged += OnCoinsChanged;
        var relinker = storage.GetComponent<PlayerMineRelinker>();
        relinker.OnResourceIdsChanged += OnResourceIdsChanged;
    }

    private void OnResourceIdsChanged(List<ulong> list)
    {
        resourceIds = list;
    }

    private void OnCoinsChanged(ulong newAmount)
    {
        profileUser.coins = newAmount;
        OnProfileCoinsChanged?.Invoke(profileUser);
    }
    protected override void Start()
    {
        base.Start();
        OnProfileChanged?.Invoke(profileUser);
    }
    public ProfileUser GetProfile()
    {
        return profileUser;
    }
    public void SetProfileUser(string userName)
    {
        profileUser.userName = userName;
        OnProfileChanged?.Invoke(profileUser);
    }
    public void AddFriend(string friend)
    {
        profileUser.AddFriend(friend);
    }
    public void RemoveFriend(string friend)
    {
        profileUser.RemoveFriend(friend);
    }

    public void LoadData(GameData _data)
    {
        profileUser.userName = _data.characterName;
        profileUser.coins = _data.coins;
        profileUser.userId = _data.characterId;
        profileUser.point = _data.point;
        profileUser.itemDataPoint = _data.itemDataPoint;
        OnProfileReady?.Invoke(profileUser);
    }

    public void SaveGame(ref GameData _data)
    {
        _data.mineOfflineDataList.Clear();
        _data.coins = profileUser.coins;
        _data.characterName = profileUser.userName;
        _data.point = profileUser.point;
        profileUser.itemDataPoint = _data.itemDataPoint;
        foreach (var resourceId in resourceIds)
        {
            _data.mineOfflineDataList.AddOrUpdate(resourceId, 0, 0, "");
        }
    }
}