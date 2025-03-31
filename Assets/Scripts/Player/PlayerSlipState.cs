using UnityEngine;

public class PlayerSlipState : IState
{
    private readonly StateMachine _fsm;
    private readonly InputHandler _inputHandler;
    private readonly CharacterController _controller;
    private readonly Transform _playerTransform;
    private readonly Transform _cameraTransform;
    private readonly PlayerStatus _playerStatus;
    private readonly GameObject _handsPOV;
    private readonly CooldownController _cooldownController;

    private Rigidbody _rb;
    private CapsuleCollider _capsuleColl;

    public PlayerSlipState(StateMachine fsm, InputHandler inputHandler,
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

    public void OnEnter() 
    {
        _rb = PlayerManager.Instance.GetComponent<Rigidbody>();
        _capsuleColl = PlayerManager.Instance.GetComponent<CapsuleCollider>();

        _rb.isKinematic = false;
        _capsuleColl.enabled = true;
        _controller.enabled = false;
    }
    public void OnExit() 
    {
        _rb.isKinematic = true;
        _capsuleColl.enabled = false;
        _controller.enabled = true;
    }
    public void OnFixedTick() { }
    public void OnTick()
    {

        StateChange();
    }

    private void StateChange()
    {
        Debug.DrawRay(_playerTransform.position, Vector3.down * 1.5f, Color.red);
        if (_controller.isGrounded && Physics.SphereCast(_playerTransform.position, 0.4f, Vector3.down, out RaycastHit hit, 1.5f))
        {
            if (hit.collider.gameObject.layer != LayerMask.NameToLayer("NonWalkable"))
            {
                _fsm.SetState("Idle");
            }
        }

        if (_inputHandler.jumpInput)
        {
            _fsm.SetState("Jump");
        }

    }
}
