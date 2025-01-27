using UnityEngine;

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

    public float goldCarryed;
    [SerializeField] private Vector2 goldRange;

    public float xpCarryed;
    private void Start()
    {
        currentHp = maxHp;
        goldCarryed = Random.Range((int)goldRange.x, (int)goldRange.y + 1);
    }
}
