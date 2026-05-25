using UnityEngine;

public class ShopDataPopup
{
    public string title;
    public Sprite itemIcon;
    public string type;
    public RealmType realm;
    public QuanlityType quanlity;
    public ulong price;
    public ShopDataPopup(string title, Sprite itemIcon, string type, RealmType realm, QuanlityType quanlity, ulong price)
    {
        this.title = title;
        this.itemIcon = itemIcon;
        this.type = type;
        this.realm = realm;
        this.quanlity = quanlity;
        this.price = price;
    }
    public ShopDataPopup() { }
}