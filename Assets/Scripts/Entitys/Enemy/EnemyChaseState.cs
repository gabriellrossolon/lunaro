using UnityEngine;
using UnityEngine.AI;

public class EnemyChaseState : IState
{
    private readonly StateMachine _fsm;
    private readonly NavMeshAgent _agent;
    private readonly Transform _transform;
    private readonly EnemyStats _enemyStats;
    private readonly Animator _animator;

    public EnemyChaseState(StateMachine fsm, NavMeshAgent agent,
        Transform transform,
        EnemyStats enemyStats, Animator animator)
    {
        _fsm = fsm;
        _agent = agent;
        _transform = transform;
        _enemyStats = enemyStats;
        _animator = animator;
    }

    public void OnEnter()
    {
        _agent.speed = _enemyStats.chaseSpeed;
        _animator.SetBool("isChase", true);

        int randomAttack = Random.Range(0, 3);
        _enemyStats.SelectedAttack = randomAttack;

        _enemyStats.attackRange = randomAttack switch
        {
            0 => _enemyStats.attackOneRange,
            1 => _enemyStats.attackTwoRange,
            2 => _enemyStats.attackThreeRange,
            _ => _enemyStats.attackRange
        };
    }


    public void OnExit()
    {
        _animator.SetBool("isChase", false);
    }

    public void OnFixedTick() { }

    public void OnTick()
    {
        Vector3 directionToPlayer = PlayerComponents.Instance.playerObject.transform.position - _transform.position;
        directionToPlayer.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        _transform.rotation = Quaternion.Slerp(_transform.rotation, targetRotation, Time.deltaTime * 25f);

        StateChange();

        _agent.SetDestination(PlayerComponents.Instance.playerObject.transform.position);
    }

    private void StateChange()
    {
        if (Vector3.Distance(_transform.position, PlayerComponents.Instance.playerObject.transform.position) > (_enemyStats.detectionRange * 3))
            _fsm.SetState("Patrol");

        if (Vector3.Distance(_transform.position, PlayerComponents.Instance.playerObject.transform.position) <= _enemyStats.attackRange)
            _fsm.SetState("Attack");
    }
}

