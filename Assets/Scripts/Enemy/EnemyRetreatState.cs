using UnityEngine.AI;
using UnityEngine;

public class EnemyRetreatState : IState
{
    private readonly StateMachine _fsm;
    private readonly NavMeshAgent _agent;
    private readonly Transform _transform;
    private readonly Transform _playerTransform;
    private readonly EnemyStats _enemyStats;
    private readonly Animator _animator;

    private float _retreatDistance = 7f;
    private float _retreatTimer = 0f;
    private float _randomRetreatTime;

    public EnemyRetreatState(StateMachine fsm, NavMeshAgent agent,
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
        Vector3 directionAwayFromPlayer = _transform.position - _playerTransform.position;
        Vector3 retreatPosition = _transform.position + directionAwayFromPlayer.normalized * _retreatDistance;
        
        _agent.SetDestination(retreatPosition);
        _agent.speed = _enemyStats.retreatSpeed;

        _randomRetreatTime = Random.Range(_enemyStats.retreatDistance.x, _enemyStats.retreatDistance.y);
        _retreatTimer = 0f;

        _agent.updateRotation = false;
        _animator.SetBool("isRetreating", true);
    }

    public void OnExit()
    {
        _animator.SetBool("isRetreating", false);
        _agent.updateRotation = true;
    }

    public void OnFixedTick() { }

    public void OnTick()
    {
        StateChange();

        Vector3 directionToPlayer = _playerTransform.position - _transform.position;
        directionToPlayer.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        _transform.rotation = Quaternion.Slerp(_transform.rotation, targetRotation, Time.deltaTime * 25f);

        _retreatTimer += Time.deltaTime;
    }

    private void StateChange()
    {
        if (_retreatTimer >= _randomRetreatTime)
        {
            _agent.SetDestination(_playerTransform.position);
            _fsm.SetState("Chase");
        }

        if (Vector3.Distance(_transform.position, _playerTransform.position) > _enemyStats.detectionRange)
            _fsm.SetState("Patrol");
    }
}





