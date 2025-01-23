using UnityEngine;
using UnityEngine.AI;

public class NPCIdleState : IState
{
    private readonly StateMachine _fsm;
    private readonly NavMeshAgent _agent;
    private readonly NPCFSM.NPCType _npcType;
    private readonly Transform _transform;
    private readonly NPCBehaviour _npcBehaviour;

    private float _idleDuration;
    private float _idleStartTime;
    private float _distanceToPlayer;

    public NPCIdleState(StateMachine fsm, NavMeshAgent agent, 
        NPCFSM.NPCType npcType, Transform transform,
        NPCBehaviour npcBehaviour)
    {
        _fsm = fsm;
        _agent = agent;
        _npcType = npcType;
        _transform = transform;
        _npcBehaviour = npcBehaviour;
    }

    public void OnEnter()
    {
        if (_npcType == NPCFSM.NPCType.Wanderer)
        {
            _idleDuration = Random.Range(2f, 4f);
            _idleStartTime = Time.time;
        }
        _agent.isStopped = true;
    }

    public void OnExit()
    {
        _agent.isStopped = false;
    }

    public void OnFixedTick() { }

    public void OnTick()
    {
        _distanceToPlayer = Vector3.Distance(_transform.position, PlayerManager.Instance.transform.position);

        ChangeState();
    }

    private void ChangeState()
    {
        if (_npcType == NPCFSM.NPCType.Wanderer && Time.time - _idleStartTime >= _idleDuration && _distanceToPlayer > _npcBehaviour.detectionRange)
            _fsm.SetState("Patrol");

        if (_distanceToPlayer <= _npcBehaviour.detectionRange)
            _fsm.SetState("LookAt");
    }
}
