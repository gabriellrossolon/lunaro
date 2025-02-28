using UnityEngine;

public class GreatAxeBehavior : MonoBehaviour
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

    public void ExecuteGreatAxeAttack(bool canAttack, float resourceCost, Animator weaponAnim)
    {
        if (canAttack && playerStatus.currentStamina >= resourceCost)
        {
            switch (attackSequence)
            {
                case 1:
                    weaponAnim.SetTrigger("greatAxeAttack1");
                    attackSequence++;
                    break;
                case 2:
                    weaponAnim.SetTrigger("greatAxeAttack2");
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
    public void ExecuteGreatAxeDashAttack(bool canAttack, float resourceCost, Animator weaponAnim)
    {
        if (canAttack && playerStatus.currentStamina >= resourceCost)
        {
            weaponAnim.SetTrigger("greatAxeDashAttack");
            SFXManager.Instance.playerAttacksSource.pitch = Random.Range(0.5f, 0.7f);
            SFXManager.Instance.playerAttacksSource.PlayOneShot(SFXManager.Instance.attackSwooshSound);
        }
    }

    public void ExecuteGreatAxeJumpAttack(bool canAttack, float resourceCost, Animator weaponAnim)
    {
        if (canAttack && playerStatus.currentStamina >= resourceCost)
        {
            weaponAnim.SetTrigger("greatAxeJumpAttack");
            SFXManager.Instance.playerAttacksSource.pitch = Random.Range(0.5f, 0.7f);
            SFXManager.Instance.playerAttacksSource.PlayOneShot(SFXManager.Instance.attackSwooshSound);
        }
    }

    public void UseFissure(bool chargedAttack, float resourceCost, bool canAttack, Animator weaponAnim)
    {
        if (!cooldownController.CanUseSkill()) return;

        if (chargedAttack && playerStatus.currentStamina >= resourceCost && canAttack) //A propria animação controla se canAttack ou não
        {
            weaponAnim.SetTrigger("greatAxeSpecial");
            cooldownController.StartSkillCooldown();
        }
        chargedAttack = false;
    }
    public void CallFissurePrefab() //Quem instancia o Earthquake (Especial do GreatHammer) é a propria animation;
    {
        SFXManager.Instance.playerAttacksSource.pitch = Random.Range(1f, 1.2f);
        SFXManager.Instance.playerAttacksSource.PlayOneShot(SFXManager.Instance.heavyImpactSound);

        GameObject currentFissure = Instantiate(
        weaponSkillPrefab,
        transform.position + transform.forward * 1f - new Vector3(0, 0.7f, 0),
        Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0)
        );


        var fissureBeh = currentFissure.GetComponent<FissureBehavior>();

        fissureBeh.fissureDamage = damage;
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
