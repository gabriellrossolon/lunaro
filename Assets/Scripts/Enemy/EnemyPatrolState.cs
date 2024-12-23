using UnityEngine.AI;
using UnityEngine;

public class EnemyPatrolState : IState
{
    private readonly StateMachine _fsm;
    private readonly NavMeshAgent _agent;
    private readonly Transform _transform;
    private readonly Transform _playerTransform;
    private EnemyStats _enemyStats;

    private Vector3 _spawnPosition;
    private float _destinationThreshold = 1f;

    public EnemyPatrolState(StateMachine fsm, NavMeshAgent agent, 
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
        _spawnPosition = _transform.position;
        SetRandomDestination();
    }

    public void OnExit() { }

    public void OnFixedTick() { }

    public void OnTick()
    {
        StateChange();

        if (Vector3.Distance(_transform.position, _playerTransform.position) <= _enemyStats.detectionRange)  // Usa a referência do jogador
        {
            _fsm.SetState("Chase");
        }

        if (!_agent.pathPending && _agent.remainingDistance <= _destinationThreshold)
        {
            SetRandomDestination();
        }
    }

    private void StateChange() { }

    private void SetRandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * _enemyStats.patrolRadius;
        randomDirection += _spawnPosition;
        randomDirection.y = _spawnPosition.y;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, _enemyStats.patrolRadius, NavMesh.AllAreas))
        {
            _agent.SetDestination(hit.position);
        }
    }
}
