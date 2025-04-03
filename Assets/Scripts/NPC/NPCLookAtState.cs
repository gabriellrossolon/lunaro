using UnityEngine;
using UnityEngine.AI;

public class NPCLookAtState : IState
{
    private readonly StateMachine _fsm;
    private readonly NavMeshAgent _agent;
    private readonly NPCFSM.NPCType _npcType;
    private readonly Transform _transform;
    private readonly NPCBehaviour _npcBehaviour;
    private readonly Animator _animator;
    private readonly AudioSource _npcSoundSource;

    private float _idleDuration;
    private float _idleStartTime;
    private float _distanceToPlayer;

    public NPCLookAtState(StateMachine fsm, NavMeshAgent agent,
        NPCFSM.NPCType npcType, Transform transform,
        NPCBehaviour npcBehaviour, Animator animator,
        AudioSource npcSoundSource)
    {
        _fsm = fsm;
        _agent = agent;
        _npcType = npcType;
        _transform = transform;
        _npcBehaviour = npcBehaviour;
        _animator = animator;   
        _npcSoundSource = npcSoundSource;
    }

    public void OnEnter()
    {
        _agent.isStopped = true;
        _animator.SetTrigger("salute");
        _npcSoundSource.PlayOneShot(_npcBehaviour.helloVoice);
    }

    public void OnExit()
    {
        _agent.isStopped = false;
    }

    public void OnFixedTick() { }

    public void OnTick()
    {
        _distanceToPlayer = Vector3.Distance(_transform.position, PlayerComponents.Instance.playerObject.transform.position);

        LookAtPlayer();
        ChangeState();
    }

    private void LookAtPlayer()
    {
        if (_distanceToPlayer <= _npcBehaviour.detectionRange)
        {
            Vector3 directionToPlayer = (PlayerComponents.Instance.playerObject.transform.position - _transform.position).normalized;
            directionToPlayer.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
            _transform.rotation = Quaternion.Slerp(_transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    private void ChangeState()
    {
        if (_distanceToPlayer > _npcBehaviour.detectionRange)
            _fsm.SetState("Idle");
    }
}
