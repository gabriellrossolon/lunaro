using UnityEngine.AI;
using UnityEngine;

public class EnemyFSM : MonoBehaviour
{
    private StateMachine _fsm;
    private NavMeshAgent _agent;
    private Transform _transform;
    private Transform _playerTransform;
    private EnemyStats _enemyStats;
    private Animator _animator;
    private EnemyWeaponBehavior _enemyWeaponBehavior;
    private EnemyEffects _enemyEffects;
    private AudioSource _enemySoundSource;

    public StateMachine FSM => _fsm;

    [Header("Debug")]
    public string _currentState;
    public string _previousState;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _transform = this.transform;
        _playerTransform = PlayerManager.Instance.transform;
        _enemyStats = GetComponent<EnemyStats>();
        _animator = GetComponent<Animator>();
        _enemyWeaponBehavior = GetComponentInChildren<EnemyWeaponBehavior>();
        _enemyEffects = GetComponent<EnemyEffects>();
        _enemySoundSource = GetComponent<AudioSource>();

        _fsm = new();
        _fsm.AddState("Idle", new EnemyIdleState(_fsm, _animator, _transform, _playerTransform, _enemyStats, _agent));
        _fsm.AddState("Patrol", new EnemyPatrolState(_fsm, _agent, _transform, _playerTransform, _enemyStats, _animator));
        _fsm.AddState("Chase", new EnemyChaseState(_fsm, _agent, _transform, _playerTransform, _enemyStats, _animator));
        _fsm.AddState("Attack", new EnemyAttackState(_fsm, _agent, _transform, _playerTransform, _enemyStats, _animator, _enemyEffects, _enemySoundSource));
        _fsm.AddState("Retreat", new EnemyRetreatState(_fsm, _agent, _transform, _playerTransform, _enemyStats, _animator));
        _fsm.AddState("Death", new EnemyDeathState(_fsm, _animator, _transform, _playerTransform, _enemyStats, _agent, _enemyEffects));

        _fsm.SetInitialState("Patrol");
    }

    private void Update()
    {
        UpdateCurrentState();
        _fsm.Tick();
    }

    private void FixedUpdate()
    {
        _fsm.FixedTick();
    }

    private void UpdateCurrentState()
    {
        _currentState = _fsm.CurrentState?.GetType().Name ?? "None";
        _previousState = _fsm.PreviousState?.GetType().Name ?? "None";
    }

    //Commons
    public void DoDamage(float damage)
    {
        _enemyStats.currentHp -= damage;

        if (_enemyStats.currentHp <= 0) { _fsm.SetState("Death"); }
    }

    public void Retreat()
    {
        if (_enemyStats.currentHp > 0)
        {
            _fsm.SetState("Retreat");
        }
    }

    public void ActiveWeaponCollider()
    {
        _enemyWeaponBehavior.CallCollider();
    }

    public void ReableBlockedAttack()
    {
        EnemyWeaponBehavior[] weaponBehaviors = GetComponentsInChildren<EnemyWeaponBehavior>();

        foreach (var weaponBehavior in weaponBehaviors)
        {
            weaponBehavior.attackBlockedByPlayer = false;
        }
    }

    public void CallProjectile()
    {
        _enemyWeaponBehavior.CallProjectile();
    }
}


