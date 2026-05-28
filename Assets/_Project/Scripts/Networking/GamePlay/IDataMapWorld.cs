using System;

public interface IDataMapWorld
{
    bool IsDataReady();
    event Action<ItemData> OnDataReady;
    ItemData GetData();
}
