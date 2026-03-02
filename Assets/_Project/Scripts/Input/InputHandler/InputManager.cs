

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
    public void Awake()
    {
        inputHandler = new InputHandler();
        inputHandler.UI.Enter.performed += (InputAction.CallbackContext context) => { OnEnterClick?.Invoke(); };
    }

    #region Player
    public Vector2 GetInputDirection()
    {
        return inputHandler.Player.Move.ReadValue<Vector2>();
    }
    public bool IsPointerPressed()
    {
        return inputHandler.Player.PointerPress.IsPressed();
    }

    public Vector2 GetPointerPosition()
    {
        return inputHandler.Player.PointerPosition.ReadValue<Vector2>();
    }
    #endregion

    #region UI
    public Vector2 GetUIInputDirection()
    {
        return inputHandler.UI.Mouse.ReadValue<Vector2>();
    }
    public Vector2 GetInputScrollWheel()
    {
        return inputHandler.UI.Scroll.ReadValue<Vector2>();
    }

    public Vector2 GetUIPointerDelta()
    {
        return inputHandler.UI.PointerDelta.ReadValue<Vector2>();
    }
    public bool IsUIPointerPressed()
    {
        return inputHandler.UI.PointerPress.IsPressed();
    }

    public Vector2 GetUIPointerPosition()
    {
        return inputHandler.UI.PointerPosition.ReadValue<Vector2>();
    }
    #endregion

    #region Toggle Player

    public void TurnOnPlayerInput()
    {
        inputHandler.Player.Enable();
    }
    public void TurnOffPlayerInput()
    {
        inputHandler.Player.Disable();
    }
    #endregion

    #region Toggle UI
    public void TurnOnUIInput()
    {
        inputHandler.UI.Enable();
    }
    public void TurnOffUIInput()
    {
        inputHandler.UI.Disable();
    }

    #endregion
    private void OnEnable()
    {
        inputHandler.Enable();
    }
    private void OnDisable()
    {
        inputHandler.Disable();
    }
}