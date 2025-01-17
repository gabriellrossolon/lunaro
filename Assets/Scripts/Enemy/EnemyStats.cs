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
    public Vector2 retreatDistance;

    public float attackDamage;

    public float maxHp;
    public float currentHp;
    private void Start()
    {
        currentHp = maxHp;
    }
}
