using UnityEngine;
using UnityEngine.AI;

public class NPCPatrolState : IState
{
    private readonly StateMachine _fsm;
    private readonly Transform _transform;
    private readonly NavMeshAgent _agent;
    private readonly NPCBehaviour _npcBehaviour;
    private readonly Animator _animator;

    private Vector3 _spawnPosition;
    private float _destinationThreshold = 1f;

    private float _patrolDuration;
    private float _patrolStartTime;

    private float _distanceToPlayer;

    public NPCPatrolState(StateMachine fsm, Transform transform, 
        NavMeshAgent agent, NPCBehaviour npcBehavior, 
        Animator animator)
    {
        _fsm = fsm;
        _transform = transform;
        _agent = agent;
        _npcBehaviour = npcBehavior;
        _animator = animator;
    }

    public void OnEnter()
    {
        _agent.speed = _npcBehaviour.patrolSpeed;
        _spawnPosition = _transform.position;
        SetRandomDestination();
        _patrolDuration = Random.Range(10f, 15f);
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
        {
            SetRandomDestination();
        }

        _distanceToPlayer = Vector3.Distance(_transform.position, PlayerManager.Instance.transform.position);

        StateChange();
    }

    private void SetRandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * _npcBehaviour.patrolRadius;
        randomDirection += _spawnPosition;
        randomDirection.y = _spawnPosition.y;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, _npcBehaviour.patrolRadius, NavMesh.AllAreas))
        {
            _agent.SetDestination(hit.position);
        }
    }

    private void StateChange()
    {
        if (Time.time - _patrolStartTime >= _patrolDuration)
            _fsm.SetState("Idle");

        if (_distanceToPlayer <= _npcBehaviour.detectionRange)
            _fsm.SetState("LookAt");
    }
}