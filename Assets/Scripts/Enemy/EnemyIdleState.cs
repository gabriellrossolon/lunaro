using UnityEngine;

public class EnemyIdleState : IState
{
    private readonly StateMachine _fsm;

    public EnemyIdleState(StateMachine fsm)
    {
        _fsm = fsm;
    }

    public void OnEnter()
    {
        _fsm.SetState("Patrol");
    }
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
