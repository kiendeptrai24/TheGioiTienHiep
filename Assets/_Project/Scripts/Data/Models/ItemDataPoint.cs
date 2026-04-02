

using Newtonsoft.Json;
using UnityEngine;
[System.Serializable]
public class ItemDataPoint : ItemData
{
    public int damagePoint;
    public int defensePoint;
    public int healthPoint;
    public int manaPoint;
    public int spiritPoint;
    public int moveSpeed;
    public int spititRange;
    public override ItemData Clone()
    {
        return (ItemDataPoint)this.MemberwiseClone();
    }

}