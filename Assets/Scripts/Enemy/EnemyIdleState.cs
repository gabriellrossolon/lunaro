using UnityEngine;
using UnityEngine.UIElements;

public class EnemyIdleState : IState
{
    private readonly StateMachine _fsm;
    private readonly Animator _animator;
    private readonly Transform _transform;
    private readonly Transform _playerTransform;
    private readonly EnemyStats _enemyStats;
    
    
    private float _idleDuration;
    private float _idleStartTime;

    public EnemyIdleState(StateMachine fsm, Animator animator, 
        Transform transform, Transform playerTransform,
        EnemyStats enemyStats)
    {
        _fsm = fsm;
        _animator = animator;
        _transform = transform;
        _playerTransform = playerTransform;
        _enemyStats = enemyStats;
    }

    public void OnEnter()
    {
        _idleDuration = Random.Range(3f, 6f);
        _idleStartTime = Time.time;
        _animator.SetBool("isIdle", true);
    }

    public void OnExit() 
    {
        _animator.SetBool("isIdle", false);
    }

    public void OnFixedTick() { }

    public void OnTick()
    {
        StateChange();
    }

    private void StateChange()
    {
        if (Time.time - _idleStartTime >= _idleDuration)
        {
            _fsm.SetState("Patrol");
        }

        if (Vector3.Distance(_transform.position, _playerTransform.position) <= _enemyStats.detectionRange)
        {
            _fsm.SetState("Chase");
        }
    }
}

