using System.Collections.Generic;
using UnityEngine;

public class AccountDataPopup : IPopupData
{
    public ItemData currentProfile;
    public string createdAt;
    public string username;
    public string userId;
    public AccountDataPopup(ItemData currentProfile, string username, string userId, string createdAt)
    {
        this.currentProfile = currentProfile;
        this.username = username;
        this.userId = userId;
        this.createdAt = createdAt;
    }
    public AccountDataPopup() { }
}