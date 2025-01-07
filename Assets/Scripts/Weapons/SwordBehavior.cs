using UnityEngine;
using UnityEngine.AI;

public class SwordBehavior : MonoBehaviour
{
    private PlayerStatus playerStatus;

    public float damage;
    public bool colliderEnabled = false;

    [HideInInspector] public BoxCollider weaponColl;
    public GameObject collisionParticle;

    private void Awake()
    {
        weaponColl = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        playerStatus = PlayerManager.Instance.GetComponent<PlayerStatus>();
        playerStatus.ControlDamage(damage);
    }

    private void OnDestroy()
    {
        playerStatus.ControlDamage(-damage);
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
            other.GetComponent<EnemyFSM>().DoDamage(playerStatus.damage);
            other.GetComponent<Animator>().SetTrigger("hurt");
            Instantiate(collisionParticle, other.ClosestPointOnBounds(transform.position), Quaternion.identity);

            SFXManager.Instance.playerAttacksSource.pitch = Random.Range(0.7f, 1f);
            SFXManager.Instance.playerAttacksSource.PlayOneShot(SFXManager.Instance.fleshImpactSound);
            //Debug.Log("Colidiu com o componente: " + other.GetType().Name);
        }
    }
}