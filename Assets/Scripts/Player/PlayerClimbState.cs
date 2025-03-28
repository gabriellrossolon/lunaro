using UnityEngine;

public class PlayerClimbState : IState
{
    private readonly StateMachine _fsm;
    private readonly InputHandler _inputHandler;
    private readonly CharacterController _controller;
    private readonly Transform _playerTransform;
    private readonly PlayerStatus _playerStatus;

    public PlayerClimbState(StateMachine fsm, InputHandler inputHandler,
        CharacterController controller, Transform playerTransform,
        PlayerStatus playerStatus)
    {
        _fsm = fsm;
        _inputHandler = inputHandler;
        _controller = controller;
        _playerTransform = playerTransform;
        _playerStatus = playerStatus;
    }

    public void OnEnter()
    {
        _controller.enabled = true;
    }

    public void OnExit()
    {
        _controller.enabled = true;
    }

    public void OnFixedTick() { }

    public void OnTick()
    {
        ClimbMovement();
        StateChange();
    }

    private void ClimbMovement()
    {
        float verticalInput = _inputHandler.moveInput.y;
        Vector3 climbDirection = Vector3.up * verticalInput * _playerStatus.climbSpeed;
        _controller.Move(climbDirection * Time.deltaTime);
    }

    private void StateChange()
    {
        if (_inputHandler.jumpInput)
            _fsm.SetState("Jump");
    }
}
