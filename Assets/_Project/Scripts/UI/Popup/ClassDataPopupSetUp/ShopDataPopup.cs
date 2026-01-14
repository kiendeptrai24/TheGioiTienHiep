
public class ShopDataPopup
{
    public string title;
    public string type;
    public string realm;
    public string quality;
    public string price;
    public ShopDataPopup(string title, string type, string realm, string quality, string price)
    {
        this.title = title;
        this.type = type;
        this.realm = realm;
        this.quality = quality;
        this.price = price;
    }
    public ShopDataPopup() { }
}