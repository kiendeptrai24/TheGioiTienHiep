
using System;
using UnityEngine;

public class ProfileManager : Singleton<ProfileManager>, ISaveable
{
    [SerializeField] private ProfileUser profileUser;
    public event Action<ProfileUser> OnProfileCoinsChanged;
    public event Action<ProfileUser> OnProfileChanged;
    protected override void Awake()
    {
        base.Awake();
        string userId = Guid.NewGuid().ToString();
        profileUser = new ProfileUser(userId, "người chơi");
    }
    public void BindResource(ResourceStorage storage)
    {
        storage.OnCoinsChanged += OnCoinsChanged;
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
    public void SetProfileUser(string userId, string userName)
    {
        profileUser.userId = userId;
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
    }

    public void SaveGame(ref GameData _data)
    {
        _data.characterName = profileUser.userName;
    }
}