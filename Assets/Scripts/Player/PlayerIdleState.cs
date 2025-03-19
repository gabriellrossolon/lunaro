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
        if (_fsm.PreviousState is PlayerCombatState)
        {
            if (_handsPOV.activeSelf)
                _handsPOV.GetComponent<Animator>().SetTrigger("Exit");
        }
    }
    public void OnExit() { }
    public void OnFixedTick() { }
    public void OnTick()
    {
        if(Input.GetKey(KeyCode.R) && _handsPOV.activeSelf)
            _handsPOV.GetComponent<Animator>().SetTrigger("Exit");

        StateChange();
    }

    private void StateChange() 
    {
        if (Input.GetKeyDown(KeyCode.R) && !_handsPOV.activeSelf)
            _fsm.SetState("Combat");

        if (_inputHandler.attackInput || _inputHandler.spellInput || _inputHandler.blockInput)
            _fsm.SetState("Combat");

        if (!_controller.isGrounded)
            _fsm.SetState("Fall");

        if (_inputHandler.moveInput != Vector2.zero)
            _fsm.SetState("Move");

        if (_inputHandler.jumpInput && _controller.isGrounded)
            _fsm.SetState("Jump");

        Debug.DrawRay(_playerTransform.position, Vector3.down * 1.5f, Color.red);
        if (_controller.isGrounded && Physics.SphereCast(_playerTransform.position, 0.4f, Vector3.down, out RaycastHit hit, 1.5f))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("NonWalkable"))
            {
                _fsm.SetState("Slip");
            }
        }

    }
}
