using Unity.VisualScripting;
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
    public StateMachine FSM => _fsm;

    [Header("Public References")]
    public GameObject _handsPOV;
    public GameObject _dashParticle;

    [Header("Debug")]
    public string _currentState; 
    public string _previousState;

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

        _fsm.AddState("Idle", new PlayerIdleState(_fsm, _inputHandler, _controller, _playerTransform, _cameraTransform, _playerStatus, _handsPOV));
        _fsm.AddState("Move", new PlayerMoveState(_fsm, _inputHandler, _controller, _playerTransform, _cameraTransform, _playerStatus, _handsPOV, _cooldownController));
        _fsm.AddState("Jump", new PlayerJumpState(_fsm, _inputHandler, _controller, _playerTransform, _cameraTransform, _playerStatus, _cooldownController));
        _fsm.AddState("Fall", new PlayerFallState(_fsm, _inputHandler, _controller, _playerTransform, _cameraTransform, _playerStatus));
        _fsm.AddState("Combat", new PlayerCombatState(_fsm, _inputHandler, _controller, _playerTransform, _cameraTransform, _playerStatus, _handsPOV, _cooldownController));
        _fsm.AddState("Dash", new PlayerDashState(_fsm, _inputHandler, _controller, _playerTransform, _cooldownController, _dashParticle));

        _fsm.SetInitialState("Idle"); // Define o estado inicial
    }

    private void Update()
    {
        UpdateCurrentState();
        _fsm.Tick();

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, _cameraTransform.eulerAngles.y, transform.eulerAngles.z);
        //_controller.Move(new Vector3(0, -9.81f, 0) * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        _fsm.FixedTick();
    }

    private void UpdateCurrentState()
    {
        // Atualiza os estados atuais e anteriores para o debug
        _currentState = _fsm.CurrentState?.GetType().Name ?? "None";
        _previousState = _fsm.PreviousState?.GetType().Name ?? "None";
    }
}
