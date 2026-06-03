using UnityEngine;

public class TopDownRotation : IRotable
{
    private readonly Rigidbody rb;

    public TopDownRotation(Rigidbody rb)
    {
        this.rb = rb;
    }

    public void Rotate(Vector3 inputDirection, float rotationSpeed = 10f)
    {
        rb.angularVelocity = Vector3.zero;

        if (inputDirection.sqrMagnitude < 0.001f)
            return;

        inputDirection.Normalize();

        Quaternion targetRotation =
            Quaternion.LookRotation(inputDirection);

        Quaternion smoothRotation =
            Quaternion.Slerp(
                rb.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime);

        rb.MoveRotation(smoothRotation);
    }
}