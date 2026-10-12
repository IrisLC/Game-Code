using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class OfficePlayerInputHandler : MonoBehaviour
{
    PlayerInput.OfficePlayerActions inputActions;
    public event Action InteractPressed;
    public event Action<Vector2> LookPerformed;
    public bool MovePerformed;
    public Vector2 MoveDirection { get => inputActions.Movement.ReadValue<Vector2>(); }

    void OnEnable()
    {
        // In the event that this gets called before the GameManager's Awake(), make sure the inputs get set up.
        //  This should only occur when testing in the editor
        if (!GameManager.PlayerInputInitialized)
        {
            GameManager.SetupInputs();
        }

        inputActions = GameManager.OfficeInputs;

        inputActions.Interact.performed += OnInteract;
        inputActions.Interact.Enable();

        inputActions.WalkLook.performed += OnLook;
        inputActions.WalkLook.Enable();

        inputActions.Movement.started += OnMoveStart;
        inputActions.Movement.canceled += OnMoveEnd;
        inputActions.Movement.Enable();
    }

    // Methods for passing along the inputs to the other player scripts

    void OnInteract(InputAction.CallbackContext context)
    {
        InteractPressed?.Invoke();
    }

    void OnLook(InputAction.CallbackContext context)
    {
        LookPerformed?.Invoke(context.ReadValue<Vector2>());
    }

    void OnMoveStart(InputAction.CallbackContext context)
    {
        MovePerformed = true;
    }

    void OnMoveEnd(InputAction.CallbackContext context)
    {
        MovePerformed = false;
    }

    void OnDisable()
    {
        // Unsubscribes from all the events
        inputActions.Interact.performed -= OnInteract;
        inputActions.WalkLook.performed -= OnLook;
        inputActions.Movement.started -= OnMoveStart;
        inputActions.Movement.canceled -= OnMoveEnd;
        inputActions.WalkLook.Disable();
    }
}
