using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public Vector2 moveInput;
    public bool sprintInput;
    public bool jumpInput;
    public bool attackInput;
    public bool attackChargingInput;
    public bool leftClickIput;
    public bool blockInput;
    public bool dashInput;
    public bool spellInput;
    public bool potionInput;
    public bool inventoryInput;
    public bool pauseInput;
    public bool interactInput;

    [Header("Actions Locked?")]
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

    public void OnLeftPressed(InputAction.CallbackContext context)
    {
        if (lockAction) return;

        if (context.phase == InputActionPhase.Started)
        {
            leftClickIput = true;
        }
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
