using UnityEngine;

public class SwordBehavior : MonoBehaviour
{
    private PlayerStatus playerStatus;
    private CooldownController cooldownController;

    public float damage;
    public bool colliderEnabled = false;

    [HideInInspector] public Collider weaponColl;

    public GameObject weaponSkillPrefab;
    public float skillDamageMultiplier;

    private int attackSequence = 1;

    private GameObject activedPowerAura;
    private bool hasActivatedPower = false;


    private void Awake()
    {
        weaponColl = GetComponent<Collider>();
        cooldownController = GetComponentInParent<CooldownController>();

        activedPowerAura = transform.Find("ActivedPowerAura").gameObject;
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

    public void ActiveAirCutAura(bool chargedAttack, float resourceCost, bool canAttack, Animator weaponAnim)
    {
        if (!cooldownController.CanUseSkill()) return;

        if (chargedAttack && playerStatus.currentStamina >= resourceCost && canAttack) //A propria animação controla se canAttack ou não
        {
            weaponAnim.SetTrigger("swordSpecial");

            activedPowerAura.gameObject.SetActive(!activedPowerAura.gameObject.activeSelf);

            cooldownController.StartSkillCooldown();
        }
        chargedAttack = false;
    }
    //weaponAnim.SetTrigger("swordSpecial");

    public void CallAirCutPrefab() 
    {
        if (!activedPowerAura.activeSelf || activedPowerAura == null) return;

        SFXManager.Instance.playerAttacksSource.pitch = Random.Range(1f, 1.2f);
        SFXManager.Instance.playerAttacksSource.PlayOneShot(SFXManager.Instance.circularCutSound);

        GameObject newProjectile = Instantiate(weaponSkillPrefab, transform.position, transform.rotation);
        AirCutBehavior projectileScript = newProjectile.GetComponent<AirCutBehavior>();
        projectileScript.airCutDamage = damage * skillDamageMultiplier;
        projectileScript.SetDirection(Camera.main.transform.forward);
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