public interface IPopup
{
    void Show();
    void Hide();
    bool IsVisible { get; }
}