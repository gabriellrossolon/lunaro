using UnityEngine;

public class SwordBehavior : MonoBehaviour
{
    private PlayerStatus playerStatus;

    public float damage;
    public bool colliderEnabled = false;

    [HideInInspector] public Collider weaponColl;

    private int attackSequence = 1;

    private void Awake()
    {
        weaponColl = GetComponent<Collider>();
    }

    private void Start()
    {
        playerStatus = PlayerManager.Instance.GetComponent<PlayerStatus>();
    }

    private void Update()
    {
        weaponColl.enabled = colliderEnabled;
    }

    //Attack Variations (Only Basic Attacks)

    public void ExecuteSwordAttack(bool canAttack, float resourceCost, Animator weaponAnim, Animator leftHandAnim)
    {
        if (canAttack && playerStatus.currentStamina >= resourceCost)
        {
            switch (attackSequence)
            {
                case 1:
                    weaponAnim.SetTrigger("swordAttack1");
                    attackSequence++;
                    break;
                case 2:
                    weaponAnim.SetTrigger("swordAttack2");
                    attackSequence++;
                    break;
                case 3:
                    weaponAnim.SetTrigger("swordAttack3");
                    attackSequence++;
                    break;
                case 4:
                    weaponAnim.SetTrigger("swordAttack4");
                    attackSequence = 1;
                    break;
                default:
                    attackSequence = 1;
                    break;
            }
            string retreatTrigger = "retreat" + Random.Range(1, 4);
            leftHandAnim.SetTrigger(retreatTrigger);
        }
    }

    public void ExecuteSwordDashAttack(bool canAttack, float resourceCost, Animator weaponAnim, Animator leftHandAnim)
    {
        if (canAttack && playerStatus.currentStamina >= resourceCost)
        {
            switch (attackSequence)
            {
                case 1:
                    weaponAnim.SetTrigger("swordAttack1");
                    attackSequence++;
                    break;
                case 2:
                    weaponAnim.SetTrigger("swordAttack2");
                    attackSequence++;
                    break;
                case 3:
                    weaponAnim.SetTrigger("swordAttack3");
                    attackSequence++;
                    break;
                case 4:
                    weaponAnim.SetTrigger("swordAttack4");
                    attackSequence = 1;
                    break;
                default:
                    attackSequence = 1;
                    break;
            }
            string retreatTrigger = "retreat" + Random.Range(1, 4);
            leftHandAnim.SetTrigger(retreatTrigger);
        }
    }

    public void ExecuteSwordJumpAttack(bool canAttack, float resourceCost, Animator weaponAnim, Animator leftHandAnim)
    {
        if (canAttack && playerStatus.currentStamina >= resourceCost)
        {
            switch (attackSequence)
            {
                case 1:
                    weaponAnim.SetTrigger("swordAttack1");
                    attackSequence++;
                    break;
                case 2:
                    weaponAnim.SetTrigger("swordAttack2");
                    attackSequence++;
                    break;
                case 3:
                    weaponAnim.SetTrigger("swordAttack3");
                    attackSequence++;
                    break;
                case 4:
                    weaponAnim.SetTrigger("swordAttack4");
                    attackSequence = 1;
                    break;
                default:
                    attackSequence = 1;
                    break;
            }
            string retreatTrigger = "retreat" + Random.Range(1, 4);
            leftHandAnim.SetTrigger(retreatTrigger);
        }
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
                enemyFSM.DoDamage(damage + playerStatus.damage);

            GameManager.Instance.TriggerSlowMotion();
        }
    }
}