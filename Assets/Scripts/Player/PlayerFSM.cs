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
    public StateMachine FSM => _fsm;

    public GameObject _handsPOV;

    [Header("Debug")]
    public string _currentState; // Nova variável para armazenar o estado atual

    private void Awake()
    {
        // Inicializa a statemachine e adiciona os estados
        _fsm = new();
        _controller = GetComponent<CharacterController>();
        _inputHandler = GetComponent<InputHandler>();
        _playerTransform = transform;
        _cameraTransform = Camera.main.transform;
        _playerStatus = GetComponent<PlayerStatus>();

        _fsm.AddState("Idle", new PlayerIdleState(_fsm, _inputHandler, _controller, _playerTransform, _cameraTransform, _playerStatus, _handsPOV));
        _fsm.AddState("Move", new PlayerMoveState(_fsm, _inputHandler, _controller, _playerTransform, _cameraTransform, _playerStatus));
        _fsm.AddState("Jump", new PlayerJumpState(_fsm, _inputHandler, _controller, _playerTransform, _cameraTransform, _playerStatus));
        _fsm.AddState("Fall", new PlayerFallState(_fsm, _inputHandler, _controller, _playerTransform, _cameraTransform, _playerStatus));
        _fsm.AddState("Combat", new PlayerCombatState(_fsm, _inputHandler, _controller, _playerTransform, _cameraTransform, _playerStatus, _handsPOV));

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

        if (_fsm.CurrentState is PlayerIdleState)
        {
            _currentState = "Idle";
        }
        else if (_fsm.CurrentState is PlayerMoveState)
        {
            _currentState = "Move";
        }
        else if (_fsm.CurrentState is PlayerJumpState)
        {
            _currentState = "Jump";
        }
        else if (_fsm.CurrentState is PlayerFallState)
        {
            _currentState = "Fall";
        }
        else if (_fsm.CurrentState is PlayerCombatState)
        {
            _currentState = "Combat";
        }
    }
}