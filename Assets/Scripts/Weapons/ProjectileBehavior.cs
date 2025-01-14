using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    private PlayerStatus playerStatus;

    public float speed = 10f;
    public AudioClip projectileImpactSound;

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
        playerStatus = PlayerManager.Instance.GetComponent<PlayerStatus>();
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
            EnemyFSM enemyFSM = other.GetComponent<EnemyFSM>();
            if(enemyFSM != null) { enemyFSM.DoDamage(playerStatus.damage); }
            GameManager.Instance.TriggerSlowMotion();
            other.GetComponent<Animator>().SetTrigger("hurt");
            
        }
        if (!other.CompareTag("Player"))
        {
            GameObject collParticle = Instantiate(collisionParticle, transform.position, transform.rotation);
            collParticle.GetComponent<AudioSource>().PlayOneShot(projectileImpactSound);
            Destroy(this.gameObject);
        }
    }
}
