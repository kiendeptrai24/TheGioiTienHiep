
using System;
using System.Collections.Generic;

[Serializable]
public class ProfileUser
{
    public string userId;
    public string userName;
    public ulong coins;
    public int level;
    public int experience;
    private List<string> listFriend;
    public ProfileUser(string userId, string userName)
    {
        this.userId = userId;
        this.userName = userName;
        listFriend = new List<string>();
    }
    public ProfileUser() { }
    public void AddFriend(string friend)
    {
        listFriend.Add(friend);
    }
    public void RemoveFriend(string friend)
    {
        listFriend.Remove(friend);
    }
    public List<string> GetListFriend() => listFriend;
}