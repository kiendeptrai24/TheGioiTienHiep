

using Newtonsoft.Json;
using UnityEngine;
[System.Serializable]
public class ItemResourseData : ItemData
{
    public ResourceType resourceType;
    [JsonIgnore]
    public Vector3 position;
}