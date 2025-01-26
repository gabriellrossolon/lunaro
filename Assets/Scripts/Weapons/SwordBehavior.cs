using UnityEngine;

public class SwordBehavior : MonoBehaviour
{
    private PlayerStatus playerStatus;

    public float damage;
    public bool colliderEnabled = false;

    [HideInInspector] public Collider weaponColl;

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
            EnemyFSM enemyFSM = other.GetComponent<EnemyFSM>();
            EnemyEffects enemyFX = other.GetComponent<EnemyEffects>();
            Animator enemyAnim = other.GetComponent<Animator>();


            if (enemyAnim != null)
                enemyAnim.SetTrigger("hurt");

            if (enemyFX != null)
            {
                Instantiate(enemyFX.attackTakenParticle, other.ClosestPointOnBounds(transform.position), Quaternion.identity, other.transform);
                SFXManager.Instance.playerAttacksSource.pitch = Random.Range(0.7f, 1f);
                SFXManager.Instance.playerAttacksSource.PlayOneShot(enemyFX.attackTakenSound);
            }

            if (enemyFSM != null)
                enemyFSM.DoDamage(playerStatus.damage);

            GameManager.Instance.TriggerSlowMotion();
        }
    }
}