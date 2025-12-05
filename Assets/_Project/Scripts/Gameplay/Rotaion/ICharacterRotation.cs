using UnityEngine;

public interface ICharacterRotation
{
    void Rotate(Transform character, Vector3 inputDirection, float rotationSpeed);
}
