using UnityEngine;

public class PlayerMoveState : IState
{
    private readonly StateMachine _fsm;
    private readonly InputHandler _inputHandler;
    private readonly CharacterController _controller;
    private readonly Transform _playerTransform;
    private readonly Transform _cameraTransform;
    private readonly PlayerStatus _playerStatus;
    private readonly GameObject _handsPOV;
    private readonly CooldownController _cooldownController;

    public PlayerMoveState(StateMachine fsm, InputHandler inputHandler, 
        CharacterController controller, Transform playerTransform, 
        Transform cameraTransform, PlayerStatus playerStatus, 
        GameObject handsPOV, CooldownController cooldownController)
    {
        _fsm = fsm;
        _inputHandler = inputHandler;
        _controller = controller;
        _playerTransform = playerTransform;
        _cameraTransform = cameraTransform;
        _playerStatus = playerStatus;
        _handsPOV = handsPOV;
        _cooldownController = cooldownController;
    }

    public void OnEnter() { }
    public void OnExit() { }
    public void OnFixedTick() { }
    public void OnTick()
    {
        if (Input.GetKey(KeyCode.R) && _handsPOV.activeSelf)
            _handsPOV.GetComponent<Animator>().SetTrigger("Exit");

        Movement();
        StateChange();
    }

    private void Movement()
    {
        Vector2 input = _inputHandler.moveInput;
        Vector3 move = new Vector3(input.x, 0, input.y);
        move = _playerTransform.TransformDirection(move);
        if (_controller.enabled)
        {
            _controller.Move(move * Time.deltaTime * _playerStatus.actualSpeed);
            _controller.Move(new Vector3(0, _playerStatus.playerGravity, 0) * Time.deltaTime);
        }
    }

    private void StateChange() 
    {
        if (!_controller.isGrounded)
            _fsm.SetState("Fall");

        if (_inputHandler.moveInput == Vector2.zero)
            _fsm.SetState("Idle");

        if (_inputHandler.jumpInput && _controller.isGrounded)
            _fsm.SetState("Jump");

        if (Input.GetKeyDown(KeyCode.R) && !_handsPOV.activeSelf)
            _fsm.SetState("Combat");

        if (_inputHandler.attackInput || _inputHandler.spellInput|| _inputHandler.blockInput)
            _fsm.SetState("Combat");

        if (_inputHandler.dashInput && _cooldownController.CanUseDash())
            _fsm.SetState("Dash");

        Debug.DrawRay(_playerTransform.position, Vector3.down * 1.5f, Color.red);
        if (_controller.isGrounded && Physics.SphereCast(_playerTransform.position, 0.5f, Vector3.down, out RaycastHit hit, 1.5f))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("NonWalkable"))
            {
                _fsm.SetState("Slip");
            }
        }

    }
}
