

using System;
using UnityEngine;
using UnityEngine.InputSystem;
public enum InputType
{
    Player,
    Verhicle,
    UI,

}
public class InputManager : MonoBehaviour
{
    public InputHandler inputHandler;
    public InputType inputType;
    public Action OnEnterClick;
    public Vector2 GetInputDirection()
    {
        return inputHandler.Player.Move.ReadValue<Vector2>();
    }
    public Vector2 GetUIInputDirection()
    {
        return inputHandler.UI.Mouse.ReadValue<Vector2>();
    }
    public void Awake()
    {
        inputHandler = new InputHandler();
        inputHandler.UI.Enter.performed += (InputAction.CallbackContext context) => { OnEnterClick?.Invoke(); };
    }
    private void OnEnable()
    {
        inputHandler.Enable();
    }
    private void OnDisable()
    {
        inputHandler.Disable();
    }
}