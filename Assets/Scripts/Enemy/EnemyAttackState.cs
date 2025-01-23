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

        string[] attacks = { "attack1", "attack2", "attack3" };
        int randomAttack = Random.Range(0, attacks.Length);
        _animator.SetTrigger(attacks[randomAttack]);

        _enemySoundSource.PlayOneShot(_enemyEffects.attackSound);
    }

    public void OnExit() 
    {
        _agent.isStopped = false;
    }

    public void OnFixedTick() { }

    public void OnTick()
    {
        StateChange();
    }

    private void StateChange()
    {
        //Mudança para Estado Retreat é controlada por Event Animation
    }
}

