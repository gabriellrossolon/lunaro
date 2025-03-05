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
    public float skillDamageMultiplier;

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
        }
    }
    public void ExecuteGreatAxeDashAttack(bool canAttack, float resourceCost, Animator weaponAnim)
    {
        if (canAttack && playerStatus.currentStamina >= resourceCost)
        {
            weaponAnim.SetTrigger("greatAxeDashAttack");
        }
    }

    public void ExecuteGreatAxeJumpAttack(bool canAttack, float resourceCost, Animator weaponAnim)
    {
        if (canAttack && playerStatus.currentStamina >= resourceCost)
        {
            weaponAnim.SetTrigger("greatAxeJumpAttack");
        }
    }

    public void UseGreatAxeSkill(bool chargedAttack, float resourceCost, bool canAttack, Animator weaponAnim)
    {
        if (!cooldownController.CanUseSkill() || !chargedAttack || playerStatus.currentStamina < resourceCost || !canAttack) return;
        if (InventoryManager.Instance.equippedWeapon.weaponSkill == null)
        {
            Debug.Log("Arma não possui Skill");
            return;
        }

        var skillType = InventoryManager.Instance.equippedWeapon.weaponSkill.skillType;

        switch (skillType)
        {
            case Skill.SkillType.None:
                Debug.Log("A arma possui uma Skill, porém a Skill não possui um tipo");
                break;

            case Skill.SkillType.Fissure:
                weaponAnim.SetTrigger("greatAxeSpecial");
                cooldownController.StartSkillCooldown();
                break;
        }

        chargedAttack = false;
    }

    public void CallFissurePrefab()
    {
        SFXManager.Instance.playerAttacksSource.pitch = Random.Range(1f, 1.2f);
        SFXManager.Instance.playerAttacksSource.PlayOneShot(SFXManager.Instance.heavyImpactSound);

        Vector3 playerPosition = PlayerManager.Instance.transform.position;

        Vector3 forwardDirection = Camera.main.transform.forward;
        forwardDirection.y = 0; // Remove inclinação da câmera para manter no plano horizontal
        forwardDirection.Normalize();

        Vector3 spawnPosition = playerPosition + forwardDirection * 1.5f;
        spawnPosition.y = playerPosition.y - 0.7f; // Mantém altura fixa relativa ao jogador

        GameObject currentFissure = Instantiate(
            weaponSkillPrefab,
            spawnPosition,
            Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0)
        );

        var fissureBeh = currentFissure.GetComponent<FissureBehavior>();
        fissureBeh.fissureDamage = damage * skillDamageMultiplier;
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
