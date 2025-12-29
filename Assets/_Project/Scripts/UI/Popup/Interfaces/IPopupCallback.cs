public interface IPopupCallback<T>
{
    void OnConfirm(T result);
    void OnCancel();
}