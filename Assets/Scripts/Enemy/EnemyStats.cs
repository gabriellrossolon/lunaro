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
        GetComponent<CapsuleCollider>().enabled = false;
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<EnemyFSM>().enabled = false;
        GetComponent<NavMeshAgent>().enabled = false;
        GetComponent<Animator>().enabled = false;
        //Destroy(this.gameObject, 5f);
    }

}
