using UnityEngine;

public class GreatHammerBehavior : MonoBehaviour
{
    private PlayerStatus playerStatus;
    private CooldownController cooldownController;

    public float damage;
    public bool colliderEnabled = false;

    [HideInInspector] public Collider weaponColl;
    public GameObject collisionParticle;

    private int attackSequence = 1;

    public GameObject weaponSkillPrefab;

    private void Awake()
    {
        weaponColl = GetComponent<Collider>();
        cooldownController = GetComponentInParent<CooldownController>();
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

    public void ExecuteGreatHammerAttack(bool canAttack, float resourceCost, Animator weaponAnim)
    {
        if (canAttack && playerStatus.currentStamina >= resourceCost)
        {
            switch (attackSequence)
            {
                case 1:
                    weaponAnim.SetTrigger("greatHammerAttack1");
                    attackSequence++;
                    break;
                case 2:
                    weaponAnim.SetTrigger("greatHammerAttack2");
                    attackSequence = 1;
                    break;
                default:
                    attackSequence = 1;
                    break;
            }

            SFXManager.Instance.playerAttacksSource.pitch = Random.Range(0.5f, 0.7f);
            SFXManager.Instance.playerAttacksSource.PlayOneShot(SFXManager.Instance.attackSwooshSound);
        }
    }
    public void ExecuteGreatHammerDashAttack(bool canAttack, float resourceCost, Animator weaponAnim)
    {
        if (canAttack && playerStatus.currentStamina >= resourceCost)
        {
            weaponAnim.SetTrigger("greatHammerDashAttack");
            SFXManager.Instance.playerAttacksSource.pitch = Random.Range(0.5f, 0.7f);
            SFXManager.Instance.playerAttacksSource.PlayOneShot(SFXManager.Instance.attackSwooshSound);
        }
    }

    public void ExecuteGreatHammerJumpAttack(bool canAttack, float resourceCost, Animator weaponAnim)
    {
        if (canAttack && playerStatus.currentStamina >= resourceCost)
        {
            weaponAnim.SetTrigger("greatHammerJumpAttack");
            SFXManager.Instance.playerAttacksSource.pitch = Random.Range(0.5f, 0.7f);
            SFXManager.Instance.playerAttacksSource.PlayOneShot(SFXManager.Instance.attackSwooshSound);
        }
    }

    //Special Attacks
    public void UseEarthquake(bool chargedAttack, float resourceCost, bool canAttack, Animator weaponAnim)
    {
        if (!cooldownController.CanUseSkill()) return;

        if (chargedAttack && playerStatus.currentStamina >= resourceCost && canAttack) //A propria animação controla se canAttack ou não
        {
            weaponAnim.SetTrigger("greatHammerSpecial");
            cooldownController.StartSkillCooldown();
        }
        chargedAttack = false;
    }
    public void CallEarthquakePrefab() //Quem instancia o Earthquake (Especial do GreatHammer) é a propria animation;
    {
        SFXManager.Instance.playerAttacksSource.pitch = Random.Range(1f, 1.2f);
        SFXManager.Instance.playerAttacksSource.PlayOneShot(SFXManager.Instance.heavyImpactSound);

        GameObject currentEarthquake = Instantiate(weaponSkillPrefab, transform.position - new Vector3(0, 1, 0), Quaternion.identity);

        var explosionBeh = currentEarthquake.GetComponent<ExplosionBehavior>();

        explosionBeh.explosionDamage = damage;
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

            GameManager.Instance.TriggerSlowMotion(0.1f, 0.2f);
        }
    }
}