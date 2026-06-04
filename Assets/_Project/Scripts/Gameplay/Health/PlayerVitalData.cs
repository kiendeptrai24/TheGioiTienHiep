public class PlayerVitalData
{
    public VitalValue Health { get; } = new();
    public VitalValue Mana { get; } = new();
    public VitalValue Spirit { get; } = new();

    public VitalValue Get(VitalType type)
    {
        return type switch
        {
            VitalType.Health => Health,
            VitalType.Mana => Mana,
            VitalType.Spirit => Spirit,
            _ => Health
        };
    }
}