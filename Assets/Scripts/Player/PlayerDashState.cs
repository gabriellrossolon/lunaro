using UnityEngine;

public class PlayerDashState : IState
{
    private readonly StateMachine _fsm;
    private readonly InputHandler _inputHandler;
    private readonly CharacterController _controller;
    private readonly Transform _playerTransform;
    private readonly CooldownController _cooldownController;
    private readonly Animator _animator;
    private readonly PlayerStatus _playerStatus;
    private readonly WeaponBehavior _weaponBehavior;

    private readonly GameObject _handsPOV;

    private Vector3 dashDirection;
    private float dashTimer;
    public PlayerDashState(StateMachine fsm, InputHandler inputHandler,
        CharacterController controller, Transform playerTransform,
        CooldownController cooldownController, 
        Animator animator, GameObject handsPOV, PlayerStatus playerStatus, 
        WeaponBehavior weaponBehavior)
    {
        _fsm = fsm;
        _inputHandler = inputHandler;
        _controller = controller;
        _playerTransform = playerTransform;
        _cooldownController = cooldownController;
        _animator = animator;
        _handsPOV = handsPOV;
        _playerStatus = playerStatus;
        _weaponBehavior = weaponBehavior;
    }

    public void OnEnter()
    {
        _playerStatus.SubtractStamina(_playerStatus.dashStaminaCost);
        SFXManager.Instance.playerMovementsSource.PlayOneShot(SFXManager.Instance.dashSound);
        _handsPOV.GetComponent<Animator>().SetTrigger("dash");
        StartDash();
        //_cooldownController.canDash = false;
        _cooldownController.StartDashCooldown();
        PlayerComponents.Instance.playerManager.dashParticle.SetActive(true);
        AdjustParticleRotation();
    }

    public void OnExit()
    {
        dashTimer = 0;
        _inputHandler.dashInput = false;
    }

    public void OnTick()
    {
        PerformDash();
        StateChange();
        PerformDashAttack();
    }

    public void OnFixedTick() { }

    private void StartDash()
    {
        dashTimer = _playerStatus.dashDuration;

        Vector2 input = _inputHandler.moveInput;
        if (input.magnitude > 0)
        {
            Vector3 move = new Vector3(input.x, 0, input.y);
            dashDirection = _playerTransform.TransformDirection(move).normalized;
        }
        else
        {
            dashDirection = _playerTransform.forward;
        }
    }

    private void PerformDash()
    {
        Vector3 dashVelocity = dashDirection * _playerStatus.dashSpeed;
        _controller.Move(dashVelocity * Time.deltaTime);
    }

    private void PerformDashAttack()
    {
        if (_weaponBehavior != null)
        {
            if (_inputHandler.attackInput && !TempParent.Instance.isHoldingAnything)
            {
                _inputHandler.attackInput = false;
                _weaponBehavior.dashAttack = true;
            }

            _inputHandler.attackInput = false; //Mantem o attackInput zerado, pra não guardar Inputs de ataque.
        }
    }

    private void AdjustParticleRotation()
    {
        float targetYRotation;

        if (_inputHandler.moveInput.y > 0)
            targetYRotation = 180f;
        else if (_inputHandler.moveInput.y < 0)
            targetYRotation = 0f;
        else
            return;

        PlayerComponents.Instance.playerManager.dashParticle.transform.localRotation = Quaternion.Euler(0f, targetYRotation, 0f);
    }


    private void StateChange()
    {
        dashTimer -= Time.deltaTime;

        if (dashTimer <= 0)
        {
            if (!_controller.isGrounded)
                _fsm.SetState("Fall");
            else if (_fsm.PreviousState is PlayerCombatState)
                _fsm.SetState("Combat");
            else
                _fsm.SetState("Idle");
        }
    }
}




