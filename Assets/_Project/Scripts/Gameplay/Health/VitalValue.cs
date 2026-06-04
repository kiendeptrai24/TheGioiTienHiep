

using UnityEngine;

public class VitalValue
{
    public int Max { get; private set; }
    public int Current { get; private set; }

    public float Percent => Max <= 0 ? 0f : Mathf.Clamp01((float)Current / Max);

    public void Set(int max, int current)
    {
        Max = Mathf.Max(0, max);
        Current = Mathf.Clamp(current, 0, Max);
    }

    public void SetCurrent(int current)
    {
        Current = Mathf.Clamp(current, 0, Max);
    }

    public void Increase(int amount)
    {
        if (amount <= 0) return;
        Current = Mathf.Min(Max, Current + amount);
    }

    public void Decrease(int amount)
    {
        if (amount <= 0) return;
        Current = Mathf.Max(0, Current - amount);
    }

    public void Reset()
    {
        Current = Max;
    }
}