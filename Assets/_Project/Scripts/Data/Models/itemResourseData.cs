

using Newtonsoft.Json;
using UnityEngine;
[System.Serializable]
public class ItemResourseData : ItemData
{
    [JsonIgnore]
    public ResourceType resourceType;
    [JsonIgnore]
    public Vector3 position;
}