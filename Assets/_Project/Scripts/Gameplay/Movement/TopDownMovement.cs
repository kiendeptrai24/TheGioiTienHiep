using UnityEngine;

public class TopDownMovement : IMoveable
{
    Vector2 inputDirection = Vector2.zero;
    public void Move(Transform transform, Vector2 direction, float speed)
    {
        inputDirection = direction;
        Vector3 move = new Vector3(inputDirection.x, 0, inputDirection.y) * speed * Time.deltaTime;
        transform.position += move;
    }

    public void Jump()
    {

    }

    public bool IsMoving()
    {
        return inputDirection.magnitude > 0;
    }
}

