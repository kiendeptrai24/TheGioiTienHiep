

using Newtonsoft.Json;
using UnityEngine;
[System.Serializable]
public class ItemResourseData : ItemData
{
    [JsonIgnore]
    public ResourceSourceType resourceSourceType;
    [JsonIgnore]
    public Vector3 position;
}