using Unity.VisualScripting.Antlr3.Runtime.Misc;

public interface ISkillCaster : ISkillTarget
{
    // Resource
    float Mana { get; }
    float Stamina { get; }
    void ConsumeMana(float amount);
    void ConsumeStamina(float amount);

    // Trạng thái / tag / buff / debuff… tuỳ bạn
    bool HasState(string stateId);
    StatsData GetStats();
    ulong Id { get; }
    // Team/faction nếu cần
    int TeamId { get; }
}