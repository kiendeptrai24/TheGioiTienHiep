using UnityEngine;

public class TopDownMovement : IMoveable
{
    public TopDownMovement(Rigidbody rb)
    {
        this.rb = rb;
    }
    public Rigidbody rb;
    Vector2 inputDirection = Vector2.zero;
    public void Move(Transform transform, Vector2 direction, float speed)
    {
        Vector3 v = new Vector3(direction.x, 0f, direction.y);
        inputDirection = direction;
        if (v.sqrMagnitude > 1f) v.Normalize();

        Vector3 current = rb.linearVelocity;
        rb.linearVelocity = new Vector3(v.x * speed, current.y, v.z * speed);
    }

    public void Jump()
    {

    }

    public bool IsMoving()
    {
        return inputDirection.magnitude > .1f;
    }
}

