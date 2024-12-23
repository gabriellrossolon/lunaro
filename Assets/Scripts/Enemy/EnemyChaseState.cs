using UnityEngine;
using UnityEngine.AI;

public class EnemyChaseState : IState
{
    private readonly StateMachine _fsm;
    private readonly NavMeshAgent _agent;
    private readonly Transform _transform;
    private readonly Transform _playerTransform;
    private readonly EnemyStats _enemyStats;

    public EnemyChaseState(StateMachine fsm, NavMeshAgent agent,
        Transform transform, Transform playerTransform, 
        EnemyStats enemyStats)
    {
        _fsm = fsm;
        _agent = agent;
        _transform = transform;
        _playerTransform = playerTransform;
        _enemyStats = enemyStats;
    }

    public void OnEnter()
    {
        _agent.speed = _enemyStats.chaseSpeed;
    }

    public void OnExit() { }

    public void OnFixedTick() { }

    public void OnTick()
    {
        StateChange();

        _agent.SetDestination(_playerTransform.position);
    }

    private void StateChange() 
    {
        if (Vector3.Distance(_transform.position, _playerTransform.position) > _enemyStats.detectionRange)
        {
            _fsm.SetState("Patrol");
        }

        if (Vector3.Distance(_transform.position, _playerTransform.position) <= _enemyStats.attackRange)
        {
            _fsm.SetState("Attack");
        }
    }
}


