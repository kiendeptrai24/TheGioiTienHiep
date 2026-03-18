


using Newtonsoft.Json;
using UnityEngine;

public abstract class ItemStatsPreset : ItemPreset
{
    [Header("Damage Stats")]
    public float physicalDamage;
    public float magicalDamage;
    public float spiritDamage;
    [Header("Defense Stats")]
    public float physicalDefense;
    public float magicalDefense;
    public float spiritDefense;
}