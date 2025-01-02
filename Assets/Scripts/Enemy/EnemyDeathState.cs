using UnityEngine;
using UnityEngine.AI;

public class EnemyDeathState : IState
{
    private readonly StateMachine _fsm;
    private readonly Animator _animator;
    private readonly Transform _transform;
    private readonly Transform _playerTransform;
    private readonly EnemyStats _enemyStats;
    private readonly NavMeshAgent _agent;
    private readonly EnemyEffects _enemyEffects;

    private float _dissolveTimer = 3f;
    private float _delayTimer = 1f;
    private Material _material;
    private Color _originalColor;
    private bool _hasStartedDissolving = false;

    public EnemyDeathState(StateMachine fsm, Animator animator,
        Transform transform, Transform playerTransform,
        EnemyStats enemyStats, NavMeshAgent agent, EnemyEffects enemyEffects)
    {
        _fsm = fsm;
        _animator = animator;
        _transform = transform;
        _playerTransform = playerTransform;
        _enemyStats = enemyStats;
        _agent = agent;
        _enemyEffects = enemyEffects;

        InitializeMaterial();
    }

    public void OnEnter()
    {
        DisableComponents();
        _animator.SetTrigger("death");
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

    private void InitializeMaterial()
    {
        SkinnedMeshRenderer skinnedMeshRenderer = _transform.GetComponentInChildren<SkinnedMeshRenderer>();
        if (skinnedMeshRenderer != null)
        {
            _material = skinnedMeshRenderer.material;
            _originalColor = _material.color;
        }
        else
        {
            Debug.LogError("SkinnedMeshRenderer not found in children.");
        }
    }

    private void DisableComponents()
    {
        foreach (var component in new Component[]
        {
            _transform.GetComponent<CapsuleCollider>(),
            _transform.GetComponent<BoxCollider>(),
            _agent
        })
        {
            if (component is Behaviour behaviour)
                behaviour.enabled = false;
            else if (component is Collider collider)
                collider.enabled = false;
        }
    }

    private void HandleDelayBeforeDissolve()
    {
        if (_delayTimer > 0)
        {
            _delayTimer -= Time.deltaTime;
        }
        else
        {
            _hasStartedDissolving = true;
            _enemyEffects.deathFX.SetActive(true);
        }
    }

    private void HandleDissolveEffect()
    {
        if (_material != null)
        {
            _dissolveTimer -= Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, 1 - (_dissolveTimer / 3f));
            _material.color = new Color(_originalColor.r, _originalColor.g, _originalColor.b, alpha);

            if (_dissolveTimer <= 0)
            {
                Object.Destroy(_transform.gameObject);
            }
        }
    }
}






