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

    private float verticalVelocity;

    public PlayerFallState(StateMachine fsm, InputHandler inputHandler, 
        CharacterController controller, Transform playerTransform, 
        Transform cameraTransform, PlayerStatus playerStatus, 
        Animator animator)
    {
        _fsm = fsm;
        _inputHandler = inputHandler;
        _controller = controller;
        _playerTransform = playerTransform;
        _cameraTransform = cameraTransform;
        _playerStatus = playerStatus;
        _animator = animator;
    }

    public void OnEnter()
    {
        verticalVelocity = 0;
        _animator.SetBool("isFalling", true);
    }
    public void OnExit()
    {
        _animator.SetBool("isFalling", false);
    }
    public void OnFixedTick() { }
    public void OnTick()
    {
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
        move = _playerTransform.TransformDirection(move); // Faz com que a rotação do jogador seja a mesma da rotação da câmera
        if(_controller.enabled) { _controller.Move(move * Time.deltaTime * _playerStatus.actualSpeed); }
    }

    private void StateChange()
    {
        if (_controller.isGrounded) { _fsm.SetState("Idle"); }
    }
}
