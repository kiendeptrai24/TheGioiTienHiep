using System;

[Serializable]
public class PillData : ItemData
{
    public PillType pillType;
    public float rate;
    override public ItemData Clone()
    {
        return (PillData)this.MemberwiseClone();
    }
}