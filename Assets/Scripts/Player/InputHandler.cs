using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [HideInInspector] public Vector2 moveInput;
    [HideInInspector] public bool jumpInput;
    [HideInInspector] public bool attackInput;
    [HideInInspector] public bool blockInput;

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        float value = context.ReadValue<float>();
        jumpInput = value > 0.1f;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            attackInput = true;
        }
    }

    public void OnBlock(InputAction.CallbackContext context)
    {
        float value = context.ReadValue<float>();
        blockInput = value > 0.1f;
    }
}
