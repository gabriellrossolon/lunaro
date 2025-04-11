using UnityEngine;
using UnityEngine.AI;

public class EnemyAttackState : IState
{
    private readonly StateMachine _fsm;
    private readonly NavMeshAgent _agent;
    private readonly Transform _transform;
    private readonly EnemyStats _enemyStats;
    private readonly Animator _animator;
    private readonly EnemyEffects _enemyEffects;
    private readonly AudioSource _enemySoundSource;

    public EnemyAttackState(StateMachine fsm, NavMeshAgent agent,
        Transform transform,
        EnemyStats enemyStats, Animator animator, 
        EnemyEffects enemyEffects, AudioSource enemySoundSource)
    {
        _fsm = fsm;
        _agent = agent;
        _transform = transform;
        _enemyStats = enemyStats;
        _animator = animator;
        _enemyEffects = enemyEffects;
        _enemySoundSource = enemySoundSource;
    }

    public void OnEnter()
    {
        _agent.isStopped = true;
        _agent.velocity = Vector3.zero;

        string[] attacks = { "attack1", "attack2", "attack3" };
        _animator.SetTrigger(attacks[_enemyStats.SelectedAttack]);  // Usar o valor sorteado armazenado no enemyStats
    }

    public void OnExit() 
    {
        _agent.isStopped = false;
    }

    public void OnFixedTick() { }

    public void OnTick()
    {
        //LookAtPlayer();
        StateChange();
    }

    private void LookAtPlayer()
    {
        Vector3 directionToPlayer = (PlayerComponents.Instance.playerObject.transform.position - _transform.position).normalized;

        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
        _transform.rotation = Quaternion.Slerp(_transform.rotation, lookRotation, Time.deltaTime * 10f); // Suavizar rotação.
    }

    private void StateChange()
    {
        //Mudança para Estado Retreat é controlada por Event Animation
    }
}

