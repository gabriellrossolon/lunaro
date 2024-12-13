using UnityEngine;

public class PlayerCombatState : IState
{
    private readonly StateMachine _fsm;
    private readonly InputHandler _inputHandler;
    private readonly CharacterController _controller;
    private readonly Transform _playerTransform;
    private readonly Transform _cameraTransform;
    private readonly PlayerStatus _playerStatus;
    private readonly GameObject _handsPOV;

    private WeaponBehavior _weaponBehavior;

    public PlayerCombatState(StateMachine fsm, InputHandler inputHandler, CharacterController controller, Transform playerTransform, Transform cameraTransform, PlayerStatus playerStatus, GameObject handsPOV)
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
        _handsPOV.SetActive(true);
        _weaponBehavior = _handsPOV.GetComponentInChildren<WeaponBehavior>();
        InventoryManager.Instance.EquipWeapon();
    }
    public void OnExit() 
    {
        //_handsPOV.GetComponent<Animator>().SetTrigger("Exit");
        //_handsPOV.SetActive(false); 
    }
    public void OnFixedTick() { }
    public void OnTick()
    {
        Attack();
        Block();
        CombatMovement();
        StateChange();
    }

    private void Attack()
    {
        if (_inputHandler.attackInput)
        {
            _inputHandler.attackInput = false;
            _weaponBehavior.attack = true;
        }
    }

    private void Block()
    {
        if (_inputHandler.blockInput)
        {
            _weaponBehavior.block = true;
        }
        else
        {
            _weaponBehavior.block = false;
        }
    }

    private void CombatMovement()
    {
        Vector2 input = _inputHandler.moveInput;
        Vector3 move = new Vector3(input.x, 0, input.y);
        move = _playerTransform.TransformDirection(move); //Faz com que a rotação do jogador seja a mesma da rotação da camera
        _controller.Move(move * Time.deltaTime * _playerStatus.actualSpeed);
        _controller.Move(new Vector3(0, _playerStatus.playerGravity, 0) * Time.deltaTime);
    }

    private void StateChange()
    {
        if (Input.GetKeyDown(KeyCode.R)) _fsm.SetState("Idle");
        if (!_controller.isGrounded) _fsm.SetState("Fall");
        if (_inputHandler.jumpInput && _controller.isGrounded) { _fsm.SetState("Jump"); }
    }
}
