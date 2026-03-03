

using System;
using FeatureToggles;
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
    private FeatureManager _mgr;
    public void Awake()
    {
        inputHandler = new InputHandler();
        inputHandler.UI.Enter.performed += (InputAction.CallbackContext context) => { OnEnterClick?.Invoke(); };
        _mgr = FeatureManager.Instance;
        _mgr.OnFeatureEffectiveChanged += OnChanged;
    }

    private void OnChanged(FeatureId id, bool unlockInput)
    {
        switch (id)
        {
            case FeatureId.WorldClick_Enabled:
                if (unlockInput)
                    TurnOnPlayerInput();
                else
                    TurnOffPlayerInput();
                break;
            case FeatureId.BattleScene_Enabled:
                if (unlockInput)
                    TurnOnPlayerInput();
                else
                    TurnOffPlayerInput();
                break;
            default:
                break;
        }
    }

    #region Player
    public Vector2 GetInputDirection()
    {
        if (!inputHandler.Player.enabled) return Vector2.zero;
        return inputHandler.Player.Move.ReadValue<Vector2>();
    }
    public bool IsPointerPressed()
    {
        if (!inputHandler.Player.enabled) return false;
        return inputHandler.Player.PointerPress.IsPressed();
    }

    public Vector2 GetPointerPosition()
    {
        if (!inputHandler.Player.enabled) return Vector2.zero;
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
    public void TurnOffAllInput()
    {
        inputHandler.Disable();
    }
    public void TurnOnAllInput()
    {
        inputHandler.Enable();
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