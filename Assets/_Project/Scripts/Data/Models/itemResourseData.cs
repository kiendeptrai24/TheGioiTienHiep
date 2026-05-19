

using Newtonsoft.Json;
using UnityEngine;
[System.Serializable]
public class ItemResourseData : ItemData
{
    public string resourceId;
    public ResourceSourceType resourceSourceType;
    [JsonIgnore]
    public Vector3 position;
}