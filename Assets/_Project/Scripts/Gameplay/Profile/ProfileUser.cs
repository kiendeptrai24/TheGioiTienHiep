
using System;
using System.Collections.Generic;
using System.Numerics;

[Serializable]
public class ProfileUser
{
    public string userId;
    public string userName;
    public string createdAt;
    public int currentHealth;
    public int currentMana;
    public int currentSpirit;
    public ulong coins;
    public int level;
    public int experience;
    public int potentialPoint;
    public int skillPoint;
    public Vector3 pos;
    public Quaternion rot;
    public ItemDataPoint itemDataPoint;
    public PlayerResource playerResource;
    private List<string> listFriend;

    public ProfileUser(string userId, string userName, string createdAt)
    {
        this.userId = userId;
        this.userName = userName;
        this.createdAt = createdAt;
        pos = new Vector3(0, 0, 0);
        rot = Quaternion.Identity;
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