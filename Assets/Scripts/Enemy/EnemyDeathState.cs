using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;

public class EnemyDeathState : IState
{
    private readonly StateMachine _fsm;
    private readonly Animator _animator;
    private readonly Transform _transform;
    private readonly Transform _playerTransform;
    private readonly EnemyStats _enemyStats;
    private readonly NavMeshAgent _agent;
    private readonly EnemyEffects _enemyEffects;
    private readonly EnemyFSM _enemyFSM;

    private float _dissolveTimer = 5f;
    private float _delayTimer = 1.5f;
    private List<Material> _materials;
    private bool _hasStartedDissolving = false;


    public EnemyDeathState(StateMachine fsm, Animator animator,
        Transform transform, Transform playerTransform,
        EnemyStats enemyStats, NavMeshAgent agent, EnemyEffects enemyEffects,
        EnemyFSM enemyFSM)
    {
        _fsm = fsm;
        _animator = animator;
        _transform = transform;
        _playerTransform = playerTransform;
        _enemyStats = enemyStats;
        _agent = agent;
        _enemyEffects = enemyEffects;
        _enemyFSM = enemyFSM;

        InitializeMaterials();
    }

    public void OnEnter()
    {
        ClearAllForces();
        DisableComponents();
        _animator.SetTrigger("death");
        PlayerManager.Instance.playerStatus.ControlGold(_enemyStats.goldCarryed);
        _enemyFSM.DropXp();
    }

    public void OnExit() { }

    public void OnFixedTick() { }

    public void OnTick()
    {
        if (!_hasStartedDissolving)
        {
            HandleDelayBeforeDissolve();
        }
        else
        {
            HandleDissolveEffect();
        }
    }

    private void InitializeMaterials()
    {
        SkinnedMeshRenderer[] skinnedMeshRenderers = _transform.GetComponentsInChildren<SkinnedMeshRenderer>();

        _materials = skinnedMeshRenderers.SelectMany(renderer => renderer.materials).ToList();
    }

    private void DisableComponents()
    {
        foreach (var component in new Component[]
        {
        _transform.GetComponent<CapsuleCollider>(),
        _transform.GetComponent<BoxCollider>(),
        _transform.GetComponent<Rigidbody>(),
        _agent,
        _enemyEffects,
        _animator
        })
        {
            if (component == _animator)
            {
                if (_enemyFSM.isRagdoll)
                {
                    _animator.enabled = false;
                }
            }
            else if (component is Collider collider)
            {
                collider.enabled = false;
            }
            else if (component is Behaviour behaviour)
            {
                behaviour.enabled = false;
            }
        }
    }

    private void HandleDelayBeforeDissolve()
    {
        _delayTimer -= Time.deltaTime;
        if (_delayTimer <= 0)
        {
            _hasStartedDissolving = true;
        }
    }

    private void HandleDissolveEffect()
    {
        if (_materials != null && _materials.Count > 0)
        {
            _dissolveTimer -= Time.deltaTime;
            float dissolveAmount = Mathf.Lerp(0, 1, 1 - (_dissolveTimer / 6f));

            foreach (var material in _materials)
            {
                if (material.HasProperty("_DissolveAmount"))
                {
                    material.SetFloat("_DissolveAmount", dissolveAmount);
                }
            }

            if (dissolveAmount >= 1)
            {
                Object.Destroy(_transform.gameObject);
            }
        }
    }

    void ClearAllForces()
    {
        _agent.speed = 0;
        _agent.isStopped = true;
    }
}