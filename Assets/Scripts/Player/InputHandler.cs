using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [HideInInspector] public Vector2 moveInput;
    [HideInInspector] public bool sprintInput;
    [HideInInspector] public bool jumpInput;
    [HideInInspector] public bool attackInput;
    [HideInInspector] public bool blockInput;
    [HideInInspector] public bool dashInput;

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        float value = context.ReadValue<float>();
        sprintInput = value > 0.1f;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        float value = context.ReadValue<float>();
        jumpInput = value > 0.1f;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            attackInput = true;
    }

    public void OnBlock(InputAction.CallbackContext context)
    {
        float value = context.ReadValue<float>();
        blockInput = value > 0.1f;
    }
    public void OnDash(InputAction.CallbackContext context)
    {
        float value = context.ReadValue<float>();
        dashInput = value > 0.1f;
    }
}
