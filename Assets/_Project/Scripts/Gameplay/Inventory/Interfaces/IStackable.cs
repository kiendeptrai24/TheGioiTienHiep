

public interface IStackable
{
    int MaxStack { get; }
    int CurrentStack { get; set; }
    void Stack(int amount);
    void Unstack(int amount);
    bool CanStack(int amount);
}
