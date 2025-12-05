


using UnityEngine;

public interface Inputable {
    void ProcessInput();
    Vector2 GetInputDirection();
    void EnableInput();
    void DisableInput();
}