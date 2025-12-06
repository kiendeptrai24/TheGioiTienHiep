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
    public NetworkVariable<StatType> StatTypeNet = new NetworkVariable<StatType>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );


    [SerializeField] private float baseValue;
    public StatType statType;
    public NetworkList<int> modifiers = new NetworkList<int>(
        new List<int>(),                        // initial values
        NetworkVariableReadPermission.Everyone, // read
        NetworkVariableWritePermission.Server   // write
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
    public void SetDefaultValue(StatType _type, int value)
    {
        statType = _type;
        baseValue = value;
    }
    public void AddModifier(int _modifier)
    {
        modifiers.Add(_modifier);
    }
    public void RemoveModifier(int _modifier)
    {
        modifiers.Remove(_modifier);
    }
}