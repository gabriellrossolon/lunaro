using UnityEngine;

public class PlayerJumpState : IState
{
    private readonly StateMachine _fsm;
    private readonly InputHandler _inputHandler;
    private readonly CharacterController _controller;
    private readonly Transform _playerTransform;
    private readonly Transform _cameraTransform;
    private readonly PlayerStatus _playerStatus;

    private float verticalVelocity;

    public PlayerJumpState(StateMachine fsm, InputHandler inputHandler, CharacterController controller, Transform playerTransform, Transform cameraTransform, PlayerStatus playerStatus)
    {
        _fsm = fsm;
        _inputHandler = inputHandler;
        _controller = controller;
        _playerTransform = playerTransform;
        _cameraTransform = cameraTransform;
        _playerStatus = playerStatus;
    }

    public void OnEnter() 
    {
        ExecuteJump();
    }
    public void OnExit() { }
    public void OnFixedTick() { }
    public void OnTick()
    {
        AirMovement();
        verticalVelocity += _playerStatus.playerGravity * Time.deltaTime;
        StateChange();
    }

    private void AirMovement()
    {
        Vector2 input = _inputHandler.moveInput;
        Vector3 move = new Vector3(input.x, 0, input.y);
        move = _playerTransform.TransformDirection(move);
        Vector3 movement = move * _playerStatus.actualSpeed + new Vector3(0, verticalVelocity, 0);
        _controller.Move(movement * Time.deltaTime);
    }

    private void ExecuteJump()
    {
        verticalVelocity = Mathf.Sqrt(_playerStatus.jumpHeight * -2f * _playerStatus.playerGravity);
    }

    private void StateChange()
    {
        if (verticalVelocity <= 0) _fsm.SetState("Fall");
        if (_controller.isGrounded) { _fsm.SetState("Idle"); }
    }
}
