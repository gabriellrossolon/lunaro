using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStats : MonoBehaviour
{
    public float detectionRange;
    public float patrolRadius;
    public float attackRange;

    public float patrolSpeed;
    public float chaseSpeed;
    public float retreatSpeed;

    public float attackDamage;

    public float maxHp;
    public float currentHp;

    public GameObject impactFx;

    private void Start()
    {
        currentHp = maxHp;
    }

    public void DoDamage(float damage)
    {
        currentHp-= damage;

        if (currentHp <= 0) { Death(); }
    }

    private void Death()
    {
        CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();
        if (capsuleCollider != null)
        {
            capsuleCollider.enabled = false;
        }

        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            boxCollider.enabled = false;
        }

        EnemyFSM enemyFSM = GetComponent<EnemyFSM>();
        if (enemyFSM != null)
        {
            enemyFSM.enabled = false;
        }

        NavMeshAgent navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = false;
        }

        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.enabled = false;
        }

        // Destroy this GameObject after 5 seconds (optional)
        // Destroy(this.gameObject, 5f);
    }

}
