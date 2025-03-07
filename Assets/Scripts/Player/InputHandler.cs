using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [HideInInspector] public Vector2 moveInput;
    [HideInInspector] public bool sprintInput;
    [HideInInspector] public bool jumpInput;
    [HideInInspector] public bool attackInput;
    [HideInInspector] public bool attackChargingInput;
    [HideInInspector] public bool blockInput;
    [HideInInspector] public bool dashInput;
    [HideInInspector] public bool spellInput;
    [HideInInspector] public bool potionInput;
    [HideInInspector] public bool inventoryInput;
    [HideInInspector] public bool pauseInput;
    [HideInInspector] public bool interactInput;

    public bool lockAction = false;

    public void OnMove(InputAction.CallbackContext context)
    {
        if (lockAction)
        {
            moveInput = Vector2.zero;
            return;
        }

        moveInput = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        float value = context.ReadValue<float>();
        sprintInput = value > 0.1f;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (lockAction) return;

        float value = context.ReadValue<float>();
        jumpInput = value > 0.1f;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (lockAction) return;

        if (context.phase == InputActionPhase.Started)
            attackInput = true;
    }

    public void OnAttackCharged(InputAction.CallbackContext context)
    {
        if (lockAction) return;

        float value = context.ReadValue<float>();
        attackChargingInput = value > 0.1f;
    }

    public void OnBlock(InputAction.CallbackContext context)
    {
        if (lockAction) return;

        float value = context.ReadValue<float>();
        blockInput = value > 0.1f;
    }
    public void OnDash(InputAction.CallbackContext context)
    {
        if (lockAction) return;

        float value = context.ReadValue<float>();
        dashInput = value > 0.1f;
    }
    public void OnSpell(InputAction.CallbackContext context)
    {
        if (lockAction) return;

        float value = context.ReadValue<float>();
        spellInput = value > 0.1f;
    }
    public void OnPotion(InputAction.CallbackContext context)
    {
        if (lockAction) return;

        float value = context.ReadValue<float>();
        potionInput = value > 0.1f;
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            inventoryInput = true;
        }
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            pauseInput = true;
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            interactInput = true;
        }
    }

}
