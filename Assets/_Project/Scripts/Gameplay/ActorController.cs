

using System.Collections.Generic;
using UnityEngine;
public enum ActorState
{
    TopDown,
    FirstPerson,
    ThirdPerson
}
public class ActorController : TGTHNetworkBehaviour
{
    public ActorState currentState = ActorState.TopDown;
    [Header("Components")]
    [SerializeField] private float turnSpeed = 10f;
    [SerializeField] private float moveSpeed = 5f;
    private ICharacterRotation characterRotation;
    private Inputable inputable;
    public IMoveable moveable;
    protected override void Awake()
    {
        if (currentState == ActorState.TopDown)
        {
            characterRotation = new TopDownRotation();
            inputable = new KeyboardInput(new InputHandler());
            moveable = new TopDownMovement();
        }
    }
    private void Update()
    {
        if (!IsOwner) return;
        if (currentState == ActorState.TopDown)
            TopDownControl();
    }

    private void TopDownControl()
    {
        Vector2 inputDirection = inputable.GetInputDirection();
        characterRotation.Rotate(transform, new Vector3(inputDirection.x, 0, inputDirection.y), turnSpeed);
        moveable.Move(transform, inputDirection, moveSpeed);
    }

    private void OnEnable()
    {
        
        inputable.EnableInput();
    }
    private void OnDisable()
    {
        inputable.DisableInput();
    }
}