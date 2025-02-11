using UnityEngine;

public class PlayerDeathState : IState
{
    private readonly StateMachine _fsm;
    private readonly InputHandler _inputHandler;
    private readonly CharacterController _controller;
    private readonly Transform _playerTransform;
    private readonly Transform _cameraTransform;
    private readonly PlayerStatus _playerStatus;
    private readonly GameObject _handsPOV;

    public PlayerDeathState(StateMachine fsm, InputHandler inputHandler, 
        GameObject handsPOV)
    {
        _fsm = fsm;
        _inputHandler = inputHandler;
        _handsPOV = handsPOV;
    }

    public void OnEnter()
    {
        SaveManager.Instance.SaveGame();

        PlayerManager.Instance.playerIsDeath = true;
        HudManager.Instance.ChangeToDeathUI();

        SFXManager.Instance.miscSoundSource.PlayOneShot(SFXManager.Instance.deathSound);
        SFXManager.Instance.musicSource.Stop();

        _handsPOV.GetComponent<Animator>().SetTrigger("Exit");
    }
    public void OnExit()
    {
        PlayerManager.Instance.Ressurect();
        HudManager.Instance.ChangeToLiveUI();

        _handsPOV.SetActive(true);
    }
    public void OnFixedTick() { }
    public void OnTick()
    {
        StateChange();
    }

    private void StateChange()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            _fsm.SetState("Idle");
        }
    }
}