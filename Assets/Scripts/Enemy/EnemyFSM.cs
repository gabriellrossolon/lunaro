using UnityEngine.AI;
using UnityEngine;

public class EnemyFSM : MonoBehaviour
{
    private StateMachine _fsm;
    private NavMeshAgent _agent;
    private Transform _transform;
    private Transform _playerTransform;
    private EnemyStats _enemyStats;

    public StateMachine FSM => _fsm;

    [Header("Debug")]
    public string _currentState;
    public string _previousState;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _transform = this.transform;
        _playerTransform = GameObject.FindWithTag("Player").transform;
        _enemyStats = GetComponent<EnemyStats>();

        _fsm = new();
        _fsm.AddState("Idle", new EnemyIdleState(_fsm));  // Estado de Idle
        _fsm.AddState("Patrol", new EnemyPatrolState(_fsm, _agent, _transform, _playerTransform, _enemyStats));
        _fsm.AddState("Chase", new EnemyChaseState(_fsm, _agent, _transform, _playerTransform, _enemyStats));
        _fsm.AddState("Attack", new EnemyAttackState(_fsm, _agent, _transform, _playerTransform, _enemyStats));

        _fsm.SetInitialState("Idle");
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
}


