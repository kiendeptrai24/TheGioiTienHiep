


using UnityEngine;

public class KeyboardInput : Inputable
{
    public KeyboardInput(InputHandler inputAction)
    {
        this.inputAction = inputAction;
        ProcessInput();
    }
    private Vector2 inputDirection;
    public InputHandler inputAction;

    public Vector2 GetInputDirection()
    {
        return inputDirection;
    }

    public void ProcessInput()
    {
        inputAction.Player.Move.performed += ctx =>
        {
            inputDirection = ctx.ReadValue<Vector2>();
        };
        inputAction.Player.Move.canceled += ctx =>
        {
            inputDirection = Vector2.zero;
        };
    }

    public void EnableInput()
    {
        inputAction.Player.Enable();
    }

    public void DisableInput()
    {
        inputAction.Player.Disable();
    }
}