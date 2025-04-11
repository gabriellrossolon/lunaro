using UnityEngine.AI;
using UnityEngine;

public class NPCFSM : MonoBehaviour
{
    private StateMachine _fsm;
    private Animator _animator;
    private Transform _transform;
    private AudioSource _npcSoundSource;
    private NavMeshAgent _agent;
    private NPCBehaviour _npcBehaviour;

    public StateMachine FSM => _fsm;

    public enum NPCType
    {
        Wanderer,
        Stayer
    }

    [Header("NPC Type")]
    public NPCType _npcType;

    [Header("Debug")]
    public string _currentState;
    public string _previousState;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _transform = this.transform;
        _npcSoundSource = GetComponent<AudioSource>();
        _agent = GetComponent<NavMeshAgent>();
        _npcBehaviour = GetComponent<NPCBehaviour>();

        _fsm = new();
        _fsm.AddState("Idle", new NPCIdleState(_fsm, _agent, _npcType, _transform, _npcBehaviour));
        _fsm.AddState("Patrol", new NPCPatrolState(_fsm, _transform, _agent, _npcBehaviour, _animator));
        _fsm.AddState("LookAt", new NPCLookAtState(_fsm, _agent, _npcType, _transform, _npcBehaviour, _animator, _npcSoundSource));


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