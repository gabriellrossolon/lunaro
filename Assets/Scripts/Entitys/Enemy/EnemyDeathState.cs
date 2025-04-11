using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;

public class EnemyDeathState : IState
{
    private readonly StateMachine _fsm;
    private readonly Animator _animator;
    private readonly Transform _transform;
    private readonly EnemyStats _enemyStats;
    private readonly NavMeshAgent _agent;
    private readonly EnemyEffects _enemyEffects;
    private readonly EnemyFSM _enemyFSM;
    private readonly EnemyManager _enemyManager;

    //private float _dissolveTimer = 5f;
    //private float _delayTimer = 1.5f;
    //private List<Material> _materials;
    //private bool _hasStartedDissolving = false;

    private float timer;
    private float disableAnimTimer = 0.5f;


    public EnemyDeathState(StateMachine fsm, Animator animator,
        Transform transform,
        EnemyStats enemyStats, NavMeshAgent agent, EnemyEffects enemyEffects,
        EnemyFSM enemyFSM, EnemyManager enemyManager)
    {
        _fsm = fsm;
        _animator = animator;
        _transform = transform;
        _enemyStats = enemyStats;
        _agent = agent;
        _enemyEffects = enemyEffects;
        _enemyFSM = enemyFSM;
        _enemyManager = enemyManager;

        //InitializeMaterials();
    }

    public void OnEnter()
    {
        _enemyManager._enemyInfos.SetActive(false);
        SetupBonesForRagdoll();
        DisableComponents();
        _animator.SetTrigger("death");
        _enemyManager.DropXp();

        _enemyEffects.CallDeathParticle();

        var looteable = _enemyFSM.gameObject.GetComponentsInChildren<LooteableBehavior>(true);
        foreach (var item in looteable)
        {
            item.gameObject.layer = LayerMask.NameToLayer("Interactable");
        }

        AddMaterialGlow();
    }

    public void OnExit() { }

    public void OnFixedTick() { }

    public void OnTick()
    {
        timer += Time.deltaTime;
        //if (!_hasStartedDissolving)
        //{
        //    HandleDelayBeforeDissolve();
        //}
        //else
        //{
        //    HandleDissolveEffect();
        //}

        if (_enemyFSM.isRagdoll && _enemyFSM.hasDeathAnim)
        {
            if (timer >= disableAnimTimer)
                _animator.enabled = false;
        }

        if(_enemyFSM.isRagdoll && !_enemyFSM.hasDeathAnim)
        {
            _animator.enabled = false;
        }
    }

    //private void InitializeMaterials()
    //{
    //    SkinnedMeshRenderer[] skinnedMeshRenderers = _transform.GetComponentsInChildren<SkinnedMeshRenderer>();

    //    _materials = skinnedMeshRenderers.SelectMany(renderer => renderer.materials).ToList();
    //}

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
               
            }
            else if (component is Collider collider)
            {
                collider.enabled = false;
            }
            else if (component is Behaviour behaviour)
            {
                behaviour.enabled = false;
            }
            else if (component is Rigidbody rb)
            {
                rb.isKinematic = true;
            }
        }
    }

    //private void HandleDelayBeforeDissolve()
    //{
    //    _delayTimer -= Time.deltaTime;
    //    if (_delayTimer <= 0)
    //    {
    //        _hasStartedDissolving = true;
    //    }
    //}

    //private void HandleDissolveEffect()
    //{
    //    if (_materials != null && _materials.Count > 0)
    //    {
    //        _dissolveTimer -= Time.deltaTime;
    //        float dissolveAmount = Mathf.Lerp(0, 1, 1 - (_dissolveTimer / 6f));

    //        foreach (var material in _materials)
    //        {
    //            if (material.HasProperty("_DissolveAmount"))
    //            {
    //                material.SetFloat("_DissolveAmount", dissolveAmount);
    //            }
    //        }

    //        if (dissolveAmount >= 1)
    //        {
    //            Object.Destroy(_transform.gameObject);
    //        }
    //    }
    //}

    void SetupBonesForRagdoll()
    {
        Rigidbody[] rbs = _enemyFSM.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        _agent.speed = 0;
        _agent.isStopped = true;
    }

    public void AddMaterialGlow()
    {
        SkinnedMeshRenderer[] skinnedMeshes = _enemyFSM.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var smr in skinnedMeshes)
        {
            Material[] materials = smr.materials;
            Material[] newMaterials = new Material[materials.Length + 1];
            materials.CopyTo(newMaterials, 0);
            newMaterials[materials.Length] = _enemyEffects.glowShader;
            smr.materials = newMaterials;
        }
    }
}