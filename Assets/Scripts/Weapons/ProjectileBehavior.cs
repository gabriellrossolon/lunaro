using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    public float speed = 10f;
    [HideInInspector] public float damage;
    private Vector3 direction;
    private Rigidbody rb;
    public GameObject collisionParticle;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetDirection(Vector3 lookDirection)
    {
        direction = lookDirection.normalized;
    }

    private void Start()
    {
        Destroy(gameObject, 2f);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = direction * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyStats>().DoDamage(damage);
            Instantiate(collisionParticle, transform.position, transform.rotation);
        }

        if (!other.CompareTag("Player"))
        {
            Destroy(this.gameObject);
        }
    }
}
