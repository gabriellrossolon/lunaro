using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    public float speed = 10f;
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
            //Destroy(other.gameObject);
        }

        if (!other.CompareTag("Player"))
        {
            Instantiate(collisionParticle, transform.position, transform.rotation);
            Destroy(this.gameObject);
        }
    }
}
