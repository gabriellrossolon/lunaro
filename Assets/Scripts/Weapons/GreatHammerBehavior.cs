using UnityEngine;

public class GreatHammerBehavior : MonoBehaviour
{
    private PlayerStatus playerStatus;

    public float damage;
    public bool colliderEnabled = false;

    [HideInInspector] public Collider weaponColl;
    public GameObject collisionParticle;

    private void Awake()
    {
        weaponColl = GetComponent<Collider>();
    }

    private void Start()
    {
        playerStatus = PlayerManager.Instance.GetComponent<PlayerStatus>();
        playerStatus.ControlBaseDamage(damage);
    }

    private void OnDestroy()
    {
        if (playerStatus != null)
            playerStatus.ControlBaseDamage(-damage);
    }

    private void Update()
    {
        weaponColl.enabled = colliderEnabled;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            GameManager.Instance.TriggerSlowMotion();
            Instantiate(collisionParticle, other.ClosestPointOnBounds(transform.position), Quaternion.identity);
            other.GetComponent<EnemyFSM>().DoDamage(playerStatus.damage);
            other.GetComponent<Animator>().SetTrigger("hurt");

            SFXManager.Instance.playerAttacksSource.pitch = Random.Range(0.7f, 1f);
            SFXManager.Instance.playerAttacksSource.PlayOneShot(SFXManager.Instance.heavyImpactSound);
        }
    }
}