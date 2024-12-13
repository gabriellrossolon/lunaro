using UnityEngine;

public class PlayerIdleState : IState
{
    private readonly StateMachine _fsm;
    private readonly InputHandler _inputHandler;
    private readonly CharacterController _controller;
    private readonly Transform _playerTransform;
    private readonly Transform _cameraTransform;
    private readonly PlayerStatus _playerStatus;
    private readonly GameObject _handsPOV;

    public PlayerIdleState(StateMachine fsm, InputHandler inputHandler, CharacterController controller, Transform playerTransform, Transform cameraTransform, PlayerStatus playerStatus, GameObject handsPOV)
    {
        _fsm = fsm;
        _inputHandler = inputHandler;
        _controller = controller;
        _playerTransform = playerTransform;
        _cameraTransform = cameraTransform;
        _playerStatus = playerStatus;
        _handsPOV = handsPOV;
    }

    public void OnEnter()
    {
        if (_handsPOV.activeSelf)
        {
            _handsPOV.GetComponent<Animator>().SetTrigger("Exit");
        }
    }
    public void OnExit() { }
    public void OnFixedTick() { }
    public void OnTick()
    {
        StateChange();
    }

    private void StateChange() 
    {
        if (_inputHandler.attackInput || Input.GetKeyDown(KeyCode.R) || _inputHandler.blockInput) _fsm.SetState("Combat");
        if (!_controller.isGrounded) _fsm.SetState("Fall");
        if (_inputHandler.moveInput != Vector2.zero) { _fsm.SetState("Move"); }
        if(_inputHandler.jumpInput && _controller.isGrounded) { _fsm.SetState("Jump"); }
    }
}
