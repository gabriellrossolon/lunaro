using UnityEngine.AI;
using UnityEngine;

public class EnemyPatrolState : IState
{
    private readonly StateMachine _fsm;
    private readonly NavMeshAgent _agent;
    private readonly Transform _transform;
    private readonly Transform _playerTransform;
    private readonly EnemyStats _enemyStats;
    private readonly Animator _animator;

    private Vector3 _spawnPosition;
    private float _destinationThreshold = 1f;

    private float _patrolDuration;
    private float _patrolStartTime;

    public EnemyPatrolState(StateMachine fsm, NavMeshAgent agent,
        Transform transform, Transform playerTransform,
        EnemyStats enemyStats, Animator animator)
    {
        _fsm = fsm;
        _agent = agent;
        _transform = transform;
        _playerTransform = playerTransform;
        _enemyStats = enemyStats;
        _animator = animator;
    }

    public void OnEnter()
    {
        _agent.speed = _enemyStats.patrolSpeed;
        _spawnPosition = _transform.position;
        SetRandomDestination();
        _patrolDuration = Random.Range(10f, 30f);
        _patrolStartTime = Time.time;
        _animator.SetBool("isWalk", true);
    }

    public void OnExit() 
    {
        _animator.SetBool("isWalk", false);
    }

    public void OnFixedTick() { }

    public void OnTick()
    {
        if (!_agent.pathPending && _agent.remainingDistance <= _destinationThreshold)
            SetRandomDestination();

        StateChange();
    }

    private void SetRandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * _enemyStats.patrolRadius;
        randomDirection += _spawnPosition;
        randomDirection.y = _spawnPosition.y;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, _enemyStats.patrolRadius, NavMesh.AllAreas))
            _agent.SetDestination(hit.position);
    }

    private void StateChange()
    {
        if (Vector3.Distance(_transform.position, _playerTransform.position) <= _enemyStats.detectionRange)
            _fsm.SetState("Chase");

        if (Time.time - _patrolStartTime >= _patrolDuration)
            _fsm.SetState("Idle");
    }
}

