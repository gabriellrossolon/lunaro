using UnityEngine.AI;
using UnityEngine;
using System.Collections;

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
    private EnemyFSM _enemyFSM;
    private EnemyManager _enemyManager;

    public StateMachine FSM => _fsm;

    [Header("Debug")]
    public string _currentState;
    public string _previousState;

    public bool isRagdoll;
    public bool hasDeathAnim;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _transform = this.transform;
        _playerTransform = GameObject.FindWithTag("Player").transform;
        _enemyStats = GetComponent<EnemyStats>();
        _animator = GetComponent<Animator>();
        _enemyWeaponBehavior = GetComponentInChildren<EnemyWeaponBehavior>();
        _enemyEffects = GetComponent<EnemyEffects>();
        _enemySoundSource = GetComponent<AudioSource>();
        _enemyFSM = this;
        _enemyManager = GetComponent<EnemyManager>();

        _fsm = new();
        _fsm.AddState("Idle", new EnemyIdleState(_fsm, _animator, _transform, _playerTransform, _enemyStats, _agent));
        _fsm.AddState("Patrol", new EnemyPatrolState(_fsm, _agent, _transform, _playerTransform, _enemyStats, _animator));
        _fsm.AddState("Chase", new EnemyChaseState(_fsm, _agent, _transform, _playerTransform, _enemyStats, _animator));
        _fsm.AddState("Attack", new EnemyAttackState(_fsm, _agent, _transform, _playerTransform, _enemyStats, _animator, _enemyEffects, _enemySoundSource));
        _fsm.AddState("Retreat", new EnemyRetreatState(_fsm, _agent, _transform, _playerTransform, _enemyStats, _animator));
        _fsm.AddState("Death", new EnemyDeathState(_fsm, _animator, _transform, _playerTransform, _enemyStats, _agent, _enemyEffects, _enemyFSM, _enemyManager));

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
    

    public void Retreat()
    {
        _agent.isStopped = false;
        if (_enemyStats.currentHp > 0)
            _fsm.SetState("Retreat");
    }

    public void DoDamage(float damage)
    {
        if (_fsm.CurrentState is EnemyDeathState) return;

        _enemyStats.currentHp -= damage;

        if (_enemyStats.currentHp <= 0)
        {
            _fsm.SetState("Death");
            GameManager.Instance.TriggerSlowMotion(0.2f, 0.1f);
        }


        if (_fsm.CurrentState is EnemyIdleState || _fsm.CurrentState is EnemyPatrolState)
            _fsm.SetState("Chase");

    }
}


