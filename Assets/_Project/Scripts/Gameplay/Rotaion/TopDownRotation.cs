using UnityEngine;



public class TopDownRotation : IRotable
{
    public void Rotate(Transform character, Vector3 inputDirection, float rotationSpeed = 10f)
    {
        if (inputDirection == Vector3.zero) return;
        Quaternion targetRotation = Quaternion.LookRotation(inputDirection);
        character.rotation = Quaternion.Slerp(character.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}