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
    private readonly EnemyEffects _enemyEffects;
    private readonly AudioSource _enemySoundSource;
    private float _attackDuration = 2f; // Tempo de duração do ataque (em segundos)
    private float _attackTimer = 0f;

    public EnemyAttackState(StateMachine fsm, NavMeshAgent agent,
        Transform transform, Transform playerTransform,
        EnemyStats enemyStats, Animator animator, 
        EnemyEffects enemyEffects, AudioSource enemySoundSource)
    {
        _fsm = fsm;
        _agent = agent;
        _transform = transform;
        _playerTransform = playerTransform;
        _enemyStats = enemyStats;
        _animator = animator;
        _enemyEffects = enemyEffects;
        _enemySoundSource = enemySoundSource;
    }

    public void OnEnter()
    {
        _agent.isStopped = true;

        int randomAttack = Random.Range(0, 3); // Gera um número aleatório entre 0 (inclusive) e 2 (exclusive)
        if (randomAttack == 0)
        {
            _animator.SetTrigger("attack1");
        }
        else if (randomAttack == 1)
        {
            _animator.SetTrigger("attack2");
        }
        else
        {
            _animator.SetTrigger("attack3");
        }
        _enemySoundSource.PlayOneShot(_enemyEffects.attackSound);

        // Reinicia o contador do ataque
        _attackTimer = _attackDuration;
    }

    public void OnExit() 
    {
        _agent.isStopped = false;
    }

    public void OnFixedTick() { }

    public void OnTick()
    {
        _attackTimer -= Time.deltaTime;

        StateChange();
    }

    private void StateChange()
    {
        //if (_attackTimer <= 0f)
        //{
        //    _agent.isStopped = false;
        //    _fsm.SetState("Retreat");
        //}
    }
}

