using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[System.Serializable]
public class Stat 
{
    public NetworkVariable<float> BaseValue = new NetworkVariable<float>(
        0f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    [SerializeField] private float baseValue;
    public StatType statType;
    public List<float> modifiers = new List<float>();
    public NetworkList<float> Modifiers = new NetworkList<float>(
        new List<float>(),
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server  
    );


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
        Modifiers.Add(_modifier);
    }
    public void RemoveModifier(float _modifier)
    {
        modifiers.Remove(_modifier);
        Modifiers.Remove(_modifier);
    }
}