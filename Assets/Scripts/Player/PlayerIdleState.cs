using UnityEngine;

public class PlayerIdleState : IState
{
    private readonly StateMachine _fsm;
    private readonly InputHandler _inputHandler;
    private readonly CharacterController _controller;
    private readonly Transform _playerTransform;
    private readonly Transform _cameraTransform;
    private readonly PlayerStatus _playerStatus;

    public PlayerIdleState(StateMachine fsm, InputHandler inputHandler, CharacterController controller, Transform playerTransform, Transform cameraTransform, PlayerStatus playerStatus)
    {
        _fsm = fsm;
        _inputHandler = inputHandler;
        _controller = controller;
        _playerTransform = playerTransform;
        _cameraTransform = cameraTransform;
        _playerStatus = playerStatus;
    }

    public void OnEnter() { }
    public void OnExit() { }
    public void OnFixedTick() { }
    public void OnTick()
    {
        StateChange();
    }

    private void StateChange() 
    {
        if (_inputHandler.attackInput || Input.GetKeyDown(KeyCode.R)) _fsm.SetState("Combat");
        if (!_controller.isGrounded) _fsm.SetState("Fall");
        if (_inputHandler.moveInput != Vector2.zero) { _fsm.SetState("Move"); }
        if(_inputHandler.jumpInput && _controller.isGrounded) { _fsm.SetState("Jump"); }
    }
}
