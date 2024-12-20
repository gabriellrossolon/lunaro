using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

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
    public StateMachine FSM => _fsm;

    [Header("Public References")]
    public GameObject _handsPOV;
    public GameObject _dashParticle;

    [Header("Debug")]
    public string _currentState; 
    public string _previousState;
    private float _playerActualSpeed;

    private float velocityX;
    private float velocityZ;
    private float acceleration = 2f;
    private float deceletarion = 2f;

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

        _fsm.AddState("Idle", new PlayerIdleState(_fsm, _inputHandler, _controller, _playerTransform, _cameraTransform, _playerStatus, _handsPOV));
        _fsm.AddState("Move", new PlayerMoveState(_fsm, _inputHandler, _controller, _playerTransform, _cameraTransform, _playerStatus, _handsPOV, _cooldownController));
        _fsm.AddState("Jump", new PlayerJumpState(_fsm, _inputHandler, _controller, _playerTransform, _cameraTransform, _playerStatus, _cooldownController, _animator));
        _fsm.AddState("Fall", new PlayerFallState(_fsm, _inputHandler, _controller, _playerTransform, _cameraTransform, _playerStatus, _animator));
        _fsm.AddState("Combat", new PlayerCombatState(_fsm, _inputHandler, _controller, _playerTransform, _cameraTransform, _playerStatus, _handsPOV, _cooldownController));
        _fsm.AddState("Dash", new PlayerDashState(_fsm, _inputHandler, _controller, _playerTransform, _cooldownController, _dashParticle));

        _fsm.SetInitialState("Idle"); // Define o estado inicial
    }

    private void Update()
    {
        SpeedController();
        MovementController();
        UpdateCurrentState();
        _fsm.Tick();

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, _cameraTransform.eulerAngles.y, transform.eulerAngles.z);
    }

    private void FixedUpdate()
    {
        _fsm.FixedTick();
    }

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
            {
                targetSpeed = _playerStatus.runSpeed;
            }
            else
            {
                targetSpeed = _playerStatus.walkSpeed;
            } 
        }

        _playerActualSpeed = Mathf.Lerp(_playerActualSpeed, targetSpeed, 0.1f);

        if (Mathf.Abs(_playerActualSpeed - targetSpeed) < 0.01f)
        {
            _playerActualSpeed = targetSpeed;
        }

        _playerStatus.actualSpeed = _playerActualSpeed;
        _animator.SetFloat("speed", _playerActualSpeed);

        _animator.SetFloat("x", _inputHandler.moveInput.x);
        _animator.SetFloat("y", _inputHandler.moveInput.y);
    }

    private void MovementController()
    {
        if (Keyboard.current.wKey.isPressed && velocityZ < 0.5f && !Keyboard.current.shiftKey.isPressed)
        {
            velocityZ += Time.deltaTime * acceleration;
        }
        //if (Keyboard.current.sKey.isPressed && velocityZ > -0.5f && !Keyboard.current.shiftKey.isPressed)
        //{
        //    velocityZ -= Time.deltaTime * acceleration;
        //}
        if (Keyboard.current.aKey.isPressed && velocityX > -0.5f && !Keyboard.current.shiftKey.isPressed)
        {
            velocityX -= Time.deltaTime * acceleration;
        }
        if (Keyboard.current.dKey.isPressed && velocityX < 0.5f && !Keyboard.current.shiftKey.isPressed)
        {
            velocityX += Time.deltaTime * acceleration;
        }

        if (!Keyboard.current.wKey.isPressed && velocityZ > 0f)
        {
            velocityZ -= Time.deltaTime * deceletarion;
        }
        if(!Keyboard.current.wKey.isPressed && velocityZ < 0f)
        {
            velocityZ = 0f;
        }
        if (!Keyboard.current.aKey.isPressed && velocityX < 0f)
        {
            velocityX += Time.deltaTime * deceletarion;
        }
        if (!Keyboard.current.dKey.isPressed && velocityX > 0f)
        {
            velocityX -= Time.deltaTime * deceletarion;
        }

        if (!Keyboard.current.aKey.isPressed && !Keyboard.current.dKey.isPressed && (velocityX != 0f && velocityX > -0.05f && velocityX < 0.05f))
        {
            velocityX = 0f;
        }

        _animator.SetFloat("velocityZ", velocityZ);
        _animator.SetFloat("velocityX", velocityX);
    }

    private void UpdateCurrentState()
    {
        // Atualiza os estados atuais e anteriores para o debug
        _currentState = _fsm.CurrentState?.GetType().Name ?? "None";
        _previousState = _fsm.PreviousState?.GetType().Name ?? "None";
    }
}
