
public class FeedbackPopupData
{
    public string userId;
    public string title;
    public string message;
    public FeedbackPopupData(string userId, string title, string message)
    {
        this.userId = userId;
        this.title = title;
        this.message = message;
    }
}