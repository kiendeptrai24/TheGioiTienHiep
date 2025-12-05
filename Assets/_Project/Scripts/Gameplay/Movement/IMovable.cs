using UnityEngine;

public interface IMoveable
{

    void Move(Transform transform,Vector2 direction,float speed);
    void Jump();
    bool IsMoving();
}
