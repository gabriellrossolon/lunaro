using UnityEngine;

public class PlayerDashState : IState
{
    private readonly StateMachine _fsm;
    private readonly InputHandler _inputHandler;
    private readonly CharacterController _controller;
    private readonly Transform _playerTransform;
    private readonly CooldownController _cooldownController;
    private readonly GameObject _dashParticle;
    private readonly Animator _animator;
    private readonly PlayerStatus _playerStatus;

    private readonly GameObject _handsPOV;

    private Vector3 dashDirection;
    private float dashTimer;

    private const float dashSpeed = 25f;
    private const float dashDuration = 0.1f;

    public PlayerDashState(StateMachine fsm, InputHandler inputHandler,
        CharacterController controller, Transform playerTransform,
        CooldownController cooldownController, GameObject dashParticle, 
        Animator animator, GameObject handsPOV, PlayerStatus playerSTatus)
    {
        _fsm = fsm;
        _inputHandler = inputHandler;
        _controller = controller;
        _playerTransform = playerTransform;
        _cooldownController = cooldownController;
        _dashParticle = dashParticle;
        _animator = animator;
        _handsPOV = handsPOV;
        _playerStatus = playerSTatus;
    }

    public void OnEnter()
    {
        _playerStatus.SubtractStamina(10f);
        SFXManager.Instance.playerMovementsSource.PlayOneShot(SFXManager.Instance.dashSound);
        _handsPOV.GetComponent<Animator>().SetTrigger("dash");
        StartDash();
        _cooldownController.canDash = false;
        _dashParticle.SetActive(true);
        AdjustParticleRotation();
    }

    public void OnExit()
    {
        dashTimer = 0;
    }

    public void OnTick()
    {
        PerformDash();
        StateChange();
    }

    public void OnFixedTick() { }

    private void StartDash()
    {
        dashTimer = dashDuration;

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
        Vector3 dashVelocity = dashDirection * dashSpeed;
        _controller.Move(dashVelocity * Time.deltaTime);
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

        _dashParticle.transform.localRotation = Quaternion.Euler(0f, targetYRotation, 0f);
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




