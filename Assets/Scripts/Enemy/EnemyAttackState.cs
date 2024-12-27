using UnityEngine;
using UnityEngine.AI;

public class EnemyAttackState : IState
{
    private readonly StateMachine _fsm;
    private readonly NavMeshAgent _agent;
    private readonly Transform _transform;
    private readonly Transform _playerTransform;
    private readonly EnemyStats _enemyStats;
    private readonly Animator _animator;
    private float _attackDuration = 1f; // Tempo de duração do ataque (em segundos)
    private float _attackTimer = 0f;

    public EnemyAttackState(StateMachine fsm, NavMeshAgent agent,
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
        _agent.isStopped = true;

        _animator.SetTrigger("attack");

        // Reinicia o contador do ataque
        _attackTimer = _attackDuration;
    }

    public void OnExit() 
    {
        
    }

    public void OnFixedTick() { }

    public void OnTick()
    {
        _attackTimer -= Time.deltaTime;

        StateChange();
    }

    private void StateChange()
    {
        if (_attackTimer <= 0f)
        {
            _agent.isStopped = false;
            _fsm.SetState("Retreat");
        }
    }
}

