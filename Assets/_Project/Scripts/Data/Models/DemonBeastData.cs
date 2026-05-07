

using Newtonsoft.Json;
using UnityEngine;
[System.Serializable]
public class DemonBeastData : ItemResourseData
{
    [JsonIgnore]
    public int level;
    [JsonIgnore]
    public ulong lthach;
}