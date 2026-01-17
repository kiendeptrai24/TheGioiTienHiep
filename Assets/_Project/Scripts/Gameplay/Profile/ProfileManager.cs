
using System;
using UnityEngine;

public class ProfileManager : TGTHMonoBehaviour
{
    [SerializeField] private ProfileUser profileUser;
    public event Action<ProfileUser> OnProfileChanged;
    protected override void Awake()
    {
        base.Awake();
        string userId = Guid.NewGuid().ToString();
        profileUser = new ProfileUser(userId, "người chơi");
    }
    protected override void Start()
    {
        base.Start();
        OnProfileChanged?.Invoke(profileUser);
    }
    public ProfileUser GetProfileUser()
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

}