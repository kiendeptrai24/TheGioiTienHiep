
using System;
using System.Collections.Generic;

public interface ISaveGameData<TData, TDto>
{
    public void SaveGameData(TData data, TDto dto);
}