
using System;

[Serializable]
public class ProfileUser {
    public string userId;
    public string userName;
    public ProfileUser(string userId, string userName) {
        this.userId = userId;
        this.userName = userName;
    }
    public ProfileUser(){}
}