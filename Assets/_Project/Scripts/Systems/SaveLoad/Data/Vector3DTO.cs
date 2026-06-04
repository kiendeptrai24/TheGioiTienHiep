using System;
using UnityEngine;

[Serializable]
public struct Vector3DTO
{
    public float x;
    public float y;
    public float z;

    public Vector3DTO(Vector3 v)
    {
        x = v.x;
        y = v.y;
        z = v.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}