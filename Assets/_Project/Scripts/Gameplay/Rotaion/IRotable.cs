using UnityEngine;

public interface IRotable
{
    void Rotate(Transform character, Vector3 inputDirection, float rotationSpeed);
}
