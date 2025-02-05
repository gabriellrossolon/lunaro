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
    private readonly CooldownController _cooldownController;
    private readonly WeaponBehavior _weaponBehavior;
    private readonly SecondaryBehavior _secondaryBehavior;


    public PlayerCombatState(StateMachine fsm, InputHandler inputHandler, CharacterController controller, 
        Transform playerTransform, Transform cameraTransform, 
        PlayerStatus playerStatus, GameObject handsPOV, 
        CooldownController cooldownController, WeaponBehavior weaponBehavior,
        SecondaryBehavior secondaryBehavior)
    {
        _fsm = fsm;
        _inputHandler = inputHandler;
        _controller = controller;
        _playerTransform = playerTransform;
        _cameraTransform = cameraTransform;
        _playerStatus = playerStatus;
        _handsPOV = handsPOV;
        _cooldownController = cooldownController;
        _weaponBehavior = weaponBehavior;
        _secondaryBehavior = secondaryBehavior;
    }

    public void OnEnter() 
    {
        _handsPOV.SetActive(true);
        //InventoryManager.Instance.EquipWeapon();
    }
    public void OnExit() 
    {

    }
    public void OnFixedTick() { }
    public void OnTick()
    {
        Attack();
        AttackCharging();
        Block();
        Skill();
        CombatMovement();
        StateChange();
    }

    private void Attack()
    {
        if(_weaponBehavior != null)
        {
            if (_inputHandler.attackInput && !TempParent.Instance.isHoldingAnything)
            {
                _inputHandler.attackInput = false;
                _weaponBehavior.attack = true;
            }

            _inputHandler.attackInput = false;
        }
    }

    private void AttackCharging()
    {
        if (_weaponBehavior != null)
        {
            if (_inputHandler.attackChargingInput && !TempParent.Instance.isHoldingAnything)
            {
                _weaponBehavior.attackCharging = true;
            }
            else
            {
                _weaponBehavior.attackCharging = false;
            }
        }
    }

    private void Block()
    {
        if(_secondaryBehavior != null && _weaponBehavior != null)
        {
            if (_inputHandler.blockInput && !TempParent.Instance.isHoldingAnything)
            {
                _secondaryBehavior.blockInput = true;
                _weaponBehavior.leftBtnInput = true;
            }
            else
            {
                _secondaryBehavior.blockInput = false;
                _weaponBehavior.leftBtnInput = false;
            }
        } 
    }

    private void Skill()
    {
        if(_secondaryBehavior != null)
        {
            if(_inputHandler.skillInput && !TempParent.Instance.isHoldingAnything)
            {
                _secondaryBehavior.skillInput = true;
            }
            _inputHandler.skillInput = false;
        }
    }

    private void CombatMovement()
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
        if (Input.GetKeyDown(KeyCode.R)) 
            _fsm.SetState("Idle");
        
        if (!_controller.isGrounded) 
            _fsm.SetState("Fall");

        if (_inputHandler.jumpInput && _controller.isGrounded)
            _fsm.SetState("Jump");

        if (_inputHandler.dashInput && _cooldownController.CanUseDash() && _inputHandler.moveInput != Vector2.zero && !_weaponBehavior.leftBtnInput)
            _fsm.SetState("Dash");
    }
}
