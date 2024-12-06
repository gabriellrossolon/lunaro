using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerMoveState : IState
{
    private readonly StateMachine _fsm;
    private readonly InputHandler _inputHandler;
    private readonly CharacterController _controller;
    private readonly Transform _playerTransform;
    private readonly Transform _cameraTransform;
    private readonly PlayerStatus _playerStatus;

    public PlayerMoveState(StateMachine fsm, InputHandler inputHandler, CharacterController controller, Transform playerTransform, Transform cameraTransform, PlayerStatus playerStatus)
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
        Movement();
        StateChange();
    }

    private void Movement()
    {
        Vector2 input = _inputHandler.moveInput;
        Vector3 move = new Vector3(input.x, 0, input.y);
        move = _playerTransform.TransformDirection(move); //Faz com que a rotação do jogador seja a mesma da rotação da camera
        _controller.Move(move * Time.deltaTime * _playerStatus.actualSpeed);
        _controller.Move(new Vector3(0, _playerStatus.playerGravity, 0) * Time.deltaTime);
    }

    private void StateChange() 
    {
        if (!_controller.isGrounded) _fsm.SetState("Fall");
        if (_inputHandler.moveInput == Vector2.zero) { _fsm.SetState("Idle"); }
        if (_inputHandler.jumpInput && _controller.isGrounded) { _fsm.SetState("Jump"); }
    }
}
