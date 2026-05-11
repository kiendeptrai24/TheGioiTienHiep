

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
    public int moveSpeedPoint;
    public int spititRangePoint;
    public override ItemData Clone()
    {
        return (ItemDataPoint)this.MemberwiseClone();
    }

}