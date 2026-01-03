public interface ISkillCaster : ISkillTarget
{
    // Resource
    float Mana { get; }
    float Stamina { get; }
    void ConsumeMana(float amount);
    void ConsumeStamina(float amount);

    // Trạng thái / tag / buff / debuff… tuỳ bạn
    bool HasState(string stateId);
    

    // Team/faction nếu cần
    int TeamId { get; }
}