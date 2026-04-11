/// <summary>
/// Interface kiểm tra xem resource có đủ điều kiện hay không
/// </summary>
public interface IResourceValidator
{
    /// <summary>
    /// Kiểm tra xem PlayerResource và ItemData có đủ điều kiện hay không
    /// </summary>
    bool CanUse(PlayerResource playerResource, ItemData itemData);

    /// <summary>
    /// Lấy tên của resource
    /// </summary>
    string GetResourceName();

    /// <summary>
    /// Kiểm tra số lượng hiện tại
    /// </summary>
    int GetCurrentAmount(PlayerResource playerResource);

    /// <summary>
    /// Trừ resource khi sử dụng
    /// </summary>
    void Consume(PlayerResource playerResource, int amount);

    int GetRequiredAmount();
}
