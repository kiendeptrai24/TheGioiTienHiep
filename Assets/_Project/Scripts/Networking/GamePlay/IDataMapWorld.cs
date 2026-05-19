using System;
using UnityEditorInternal.Profiling.Memory.Experimental;

public interface IDataMapWorld
{
    bool IsDataReady();
    event Action<ItemData> OnDataReady;
    ItemData GetData();
}
