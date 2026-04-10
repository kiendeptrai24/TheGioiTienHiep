
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
    public int point;
    public ItemDataPoint itemDataPoint;
    public PlayerResource playerResource;
    private List<string> listFriend;
    public ProfileUser(string userId, string userName)
    {
        this.userId = userId;
        this.userName = userName;
        listFriend = new List<string>();
        itemDataPoint = new ItemDataPoint();
        playerResource = new PlayerResource();
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