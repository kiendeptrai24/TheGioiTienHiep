
using System;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class ProfileManager : Singleton<ProfileManager>, ISaveable
{
    [SerializeField] private ProfileUser profileUser;
    public event Action<ProfileUser> OnCoinsChanged;
    public event Action<ProfileUser> OnProfileChanged;
    protected override void Awake()
    {
        base.Awake();
        string userId = Guid.NewGuid().ToString();
        profileUser = new ProfileUser(userId, "người chơi");
    }
    public void BindResource(ResourceStorage storage)
    {
        storage.OnSpiritStoneChanged += OnSpiritStoneChanged;
    }
    public void UnbindResource(ResourceStorage storage)
    {
        storage.OnSpiritStoneChanged -= OnSpiritStoneChanged;
    }

    private void OnSpiritStoneChanged(int newAmount)
    {
        profileUser.coins = (ulong)newAmount;
        OnCoinsChanged?.Invoke(profileUser);
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
    protected override void LoadComponent()
    {
        base.LoadComponent();
    }

    public void LoadData(GameData _data)
    {
        profileUser.coins = _data.coins;
        profileUser.userName = _data.characterName;
    }

    public void SaveGame(ref GameData _data)
    {
        _data.coins = profileUser.coins;
        _data.characterName = profileUser.userName;
    }
}