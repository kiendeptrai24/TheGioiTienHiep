

using System;

public interface LoadDataInMatch 
{
    event Action<ItemData> OnDataLoaded; 
}