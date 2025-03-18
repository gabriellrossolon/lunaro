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

    public void OnEnter() { }
    public void OnExit() { }
    public void OnFixedTick() { }
    public void OnTick()
    {

        StateChange();
    }

    private void StateChange()
    {

    }
}
