using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat 
{
    [SerializeField] private float baseValue;
    public StatType statType;
    public List<float> modifiers = new List<float>();

    public Stat(){}
    public Stat(StatType _type, float _value)
    {
        statType = _type;
        baseValue = _value;
    }
    public Stat(StatType _type, Stat presetStat)
    {
        statType = _type;
        baseValue = presetStat.baseValue;
    }
    public float GetValue()
    {
        float finalValue = baseValue;
        foreach (int modifier in modifiers)
        {
            finalValue += modifier;
        }
        return finalValue;
    }
    public void SetDefaultValue(StatType _type, float value)
    {
        statType = _type;
        baseValue = value;
    }
    public void AddModifier(float _modifier)
    {
        modifiers.Add(_modifier);
    }
    public void RemoveModifier(float _modifier)
    {
        modifiers.Remove(_modifier);
    }
}