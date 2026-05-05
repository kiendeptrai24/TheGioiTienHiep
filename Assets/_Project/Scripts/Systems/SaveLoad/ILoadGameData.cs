
using System;
using System.Collections.Generic;

public interface ILoadGameData<TData, TDto>
{
    public void LoadGameData(TData data, TDto dataDto);
}