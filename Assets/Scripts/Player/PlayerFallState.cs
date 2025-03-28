using UnityEngine;

public class PlayerFallState : IState
{
    private readonly StateMachine _fsm;
    private readonly InputHandler _inputHandler;
    private readonly CharacterController _controller;
    private readonly Transform _playerTransform;
    private readonly Transform _cameraTransform;
    private readonly PlayerStatus _playerStatus;
    private readonly Animator _animator;
    private readonly WeaponBehavior _weaponBehavior;

    private float verticalVelocity;
    private float fallTimer;

    private const float minFallTimeForDamage = 1f;

    public PlayerFallState(StateMachine fsm, InputHandler inputHandler,
        CharacterController controller, Transform playerTransform,
        Transform cameraTransform, PlayerStatus playerStatus,
        Animator animator, WeaponBehavior weaponBehavior)
    {
        _fsm = fsm;
        _inputHandler = inputHandler;
        _controller = controller;
        _playerTransform = playerTransform;
        _cameraTransform = cameraTransform;
        _playerStatus = playerStatus;
        _animator = animator;
        _weaponBehavior = weaponBehavior;
    }

    public void OnEnter()
    {
        verticalVelocity = 0;
        fallTimer = 0f;
        _animator.SetBool("isFalling", true);
    }

    public void OnExit()
    {
        _animator.SetBool("isFalling", false);
    }

    public void OnFixedTick() { }

    public void OnTick()
    {
        fallTimer += Time.deltaTime;
        PerformJumpAttack();
        MoveWhileFalling();
        Falling();
        StateChange();
    }

    private void Falling()
    {
        verticalVelocity += _playerStatus.playerGravity * Time.deltaTime;
        Vector3 movement = new Vector3(0, verticalVelocity, 0);
        if (_controller.enabled) { _controller.Move(movement * Time.deltaTime); }
    }

    private void MoveWhileFalling()
    {
        Vector2 input = _inputHandler.moveInput;
        Vector3 move = new Vector3(input.x, 0, input.y);
        move = _playerTransform.TransformDirection(move);
        if (_controller.enabled) { _controller.Move(move * Time.deltaTime * _playerStatus.actualSpeed); }
    }

    private void PerformJumpAttack()
    {
        if (_weaponBehavior != null)
        {
            if (_inputHandler.attackInput && !TempParent.Instance.isHoldingAnything)
            {
                _inputHandler.attackInput = false;
                _weaponBehavior.jumpAttack = true;
            }
            _inputHandler.attackInput = false;
        }
    }

    public float GetFallDuration()
    {
        return fallTimer;
    }

    private void ApplyFallDamage(float time)
    {
        if (time < minFallTimeForDamage) return;
        float damagePercent = Mathf.Clamp01(time / 2f);
        float damage = damagePercent * _playerStatus.maxHp;
        PlayerManager.Instance.DoDamage(damage);
        Debug.Log("Dano de queda: " + damage);
    }


    private void StateChange()
    {
        if (_controller.isGrounded)
        {
            RaycastHit hit;
            if (Physics.Raycast(PlayerManager.Instance.transform.position, Vector3.down, out hit, 1.5f))
            {
                int groundLayer = hit.collider.gameObject.layer;
                string groundLayerName = LayerMask.LayerToName(groundLayer);

                if (groundLayerName == "Water")
                {
                    PlayerManager.Instance.waterLandingParticle.SetActive(true);
                    SFXManager.Instance.playerMovementsSource.PlayOneShot(SFXManager.Instance.waterLandingSound);
                }
                else
                {
                    PlayerManager.Instance.landingParticle.SetActive(true);
                    SFXManager.Instance.playerMovementsSource.PlayOneShot(SFXManager.Instance.landingSound);

                    ApplyFallDamage(GetFallDuration());
                }
            }

            if (_fsm.CurrentState is not PlayerDeathState)
            {
                _fsm.SetState("Idle");
            }
        }

        if (_inputHandler.attackInput || _inputHandler.blockInput)
            _fsm.SetState("Combat");
    }
}
