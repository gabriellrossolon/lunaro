using UnityEngine;

public class PlayerFSM : MonoBehaviour
{
    private StateMachine _fsm;
    private CharacterController _controller;
    private InputHandler _inputHandler;
    private Transform _playerTransform;
    private Transform _cameraTransform;
    private PlayerStatus _playerStatus;
    private CooldownController _cooldownController;
    private Animator _animator;
    private WeaponBehavior _weaponBehavior;
    private SecondaryBehavior _secondaryBehavior;
    public StateMachine FSM => _fsm;

    [Header("Public References")]
    public GameObject _handsPOV;
    public GameObject _dashParticle;

    [Header("Debug")]
    public string _currentState; 
    public string _previousState;
    private float _playerActualSpeed;

    private void Awake()
    {
        // Inicializa a statemachine e adiciona os estados
        _fsm = new();
        _controller = GetComponent<CharacterController>();
        _inputHandler = GetComponent<InputHandler>();
        _playerTransform = transform;
        _cameraTransform = Camera.main.transform;
        _playerStatus = GetComponent<PlayerStatus>();
        _cooldownController = GetComponent<CooldownController>();
        _animator = GetComponent<Animator>();
        _weaponBehavior = GetComponentInChildren<WeaponBehavior>();
        _secondaryBehavior = GetComponentInChildren<SecondaryBehavior>();

        _fsm.AddState("Idle", new PlayerIdleState(_fsm, _inputHandler, _controller, _playerTransform, _cameraTransform, _playerStatus, _handsPOV));
        _fsm.AddState("Move", new PlayerMoveState(_fsm, _inputHandler, _controller, _playerTransform, _cameraTransform, _playerStatus, _handsPOV, _cooldownController));
        _fsm.AddState("Jump", new PlayerJumpState(_fsm, _inputHandler, _controller, _playerTransform, _cameraTransform, _playerStatus, _cooldownController, _animator, _weaponBehavior));
        _fsm.AddState("Fall", new PlayerFallState(_fsm, _inputHandler, _controller, _playerTransform, _cameraTransform, _playerStatus, _animator, _weaponBehavior));
        _fsm.AddState("Combat", new PlayerCombatState(_fsm, _inputHandler, _controller, _playerTransform, _cameraTransform, _playerStatus, _handsPOV, _cooldownController, _weaponBehavior, _secondaryBehavior));
        _fsm.AddState("Dash", new PlayerDashState(_fsm, _inputHandler, _controller, _playerTransform, _cooldownController, _dashParticle, _animator, _handsPOV, _playerStatus, _weaponBehavior));
        _fsm.AddState("Death", new PlayerDeathState(_fsm, _inputHandler, _handsPOV));

        _fsm.SetInitialState("Idle"); // Define o estado inicial
    }
    private void Update()
    {
        SpeedController();
        UpdateCurrentState();
        _fsm.Tick();

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, _cameraTransform.eulerAngles.y, transform.eulerAngles.z);

        if(_inputHandler.moveInput != Vector2.zero) { SFXManager.Instance.UpdateStepSound(transform.position); }
    }

    private void FixedUpdate()
    {
        _fsm.FixedTick();
    }

    public bool wasSprinting;
    private void SpeedController()
    {
        float targetSpeed = 0f;

        if (_inputHandler.moveInput == Vector2.zero)
        {
            targetSpeed = 0;
            _animator.SetBool("isMoving", false);
        }
        else if (_inputHandler.moveInput != Vector2.zero)
        {
            _animator.SetBool("isMoving", true);

            if (_inputHandler.sprintInput)
                targetSpeed = _playerStatus.runSpeed;
            else
                targetSpeed = _playerStatus.walkSpeed;
        }

        if (TempParent.Instance.isHoldingAnything || _weaponBehavior.ChargingAnySkill() || _secondaryBehavior.IsBlockingAnything())
            targetSpeed /= 2f;

        _playerActualSpeed = targetSpeed;
        _playerStatus.actualSpeed = _playerActualSpeed;
    }




    private void UpdateCurrentState()
    {
        _currentState = _fsm.CurrentState?.GetType().Name ?? "None";
        _previousState = _fsm.PreviousState?.GetType().Name ?? "None";
    }
}
