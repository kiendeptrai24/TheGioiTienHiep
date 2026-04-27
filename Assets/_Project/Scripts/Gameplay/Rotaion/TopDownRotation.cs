using UnityEngine;

public class TopDownRotation : IRotable
{
    private readonly Rigidbody rb;
    public TopDownRotation(Rigidbody rb)
    {
        this.rb = rb;
    }

    public void Rotate(Transform character, Vector3 inputDirection, float rotationSpeed = 10f)
    {
        rb.angularVelocity = Vector3.zero;
        if (inputDirection == Vector3.zero) return;
        Quaternion targetRotation = Quaternion.LookRotation(inputDirection);
        character.rotation = Quaternion.Slerp(character.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}